﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using WordlePeaksShepherd.Services.Interfaces;

namespace WordlePeaksShepherd.Services;

public sealed class ShepherdService : IShepherdService
{
	private IWordService wordService;
	private ILetterService letterService;

	private string rawWords;
	public IEnumerable<string> Words => wordService.GetPotentialAnswerWords();

	private List<WordCriteria> chosenWords;
	public IEnumerable<WordCriteria> ChosenWords => chosenWords.AsReadOnly();

	private LetterRanges letterRanges;
	public LetterRanges LetterRanges => letterRanges;

	public ShepherdService(LetterRanges letterRanges, IWordService wordService, ILetterService letterService)
	{
		this.wordService = wordService;
		this.letterService = letterService;

		this.letterRanges = letterRanges;
		chosenWords = new List<WordCriteria>();

		InitializeWords();
	}

	[MemberNotNull(nameof(rawWords))]
	private void InitializeWords()
	{
		rawWords = string.Join("\n", wordService.GetPotentialAnswerWords());
	}

	public void AddWordChoice(WordCriteria wordCriteria)
	{
		chosenWords.Add(wordCriteria);

		letterService.GetWordScore(wordCriteria.LetterCriteria);
	}

	public IEnumerable<Word> GetSuggestedWords()
	{
		var firstPattern = $"[{letterRanges.First.StartRange}-{letterRanges.First.EndRange}]?";
		var secondPattern = $"[{letterRanges.Second.StartRange}-{letterRanges.Second.EndRange}]?";
		var thirdPattern = $"[{letterRanges.Third.StartRange}-{letterRanges.Third.EndRange}]?";
		var fourthPattern = $"[{letterRanges.Fourth.StartRange}-{letterRanges.Fourth.EndRange}]?";
		var fifthPattern = $"[{letterRanges.Fifth.StartRange}-{letterRanges.Fifth.EndRange}]?";
		var wordPattern = $@"^(?=.{{5}}){firstPattern}{secondPattern}{thirdPattern}{fourthPattern}{fifthPattern}$";

		var regex = new Regex(wordPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

		var matches = regex.Matches(rawWords)
			.Select(x => 
			{
				var letterCriteria = new List<LetterCriteria>
				{
					new LetterCriteria(x.Value[0], LetterStatus.Unknown, LetterRanges.First),
					new LetterCriteria(x.Value[1], LetterStatus.Unknown, LetterRanges.Second),
					new LetterCriteria(x.Value[2], LetterStatus.Unknown, LetterRanges.Third),
					new LetterCriteria(x.Value[3], LetterStatus.Unknown, LetterRanges.Fourth),
					new LetterCriteria(x.Value[4], LetterStatus.Unknown, LetterRanges.Fifth)
				};

				return new Word(x.Value, letterService.GetWordScore(letterCriteria));
			});

		return matches;
	}

	public void UndoLastWordChoice()
	{
		if(chosenWords.Count == 0)
		{
			return;
		}

		chosenWords.RemoveAt(chosenWords.Count - 1);
	}

	public void Reset()
	{
		chosenWords = new List<WordCriteria>();
		letterRanges = LetterRanges.Default;
	}
}
