using System.Diagnostics.CodeAnalysis;
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

	private List<LetterRanges> previousLetterRanges;

	public ShepherdService(LetterRanges letterRanges, IWordService wordService, ILetterService letterService)
	{
		this.wordService = wordService;
		this.letterService = letterService;

		this.letterRanges = letterRanges;
		previousLetterRanges = new List<LetterRanges>();
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

		var firstLetterRange = NarrowLetterRange(
			wordCriteria.LetterCriteria[0].Letter, wordCriteria.LetterCriteria[0].Status, letterRanges.First);
		var secondLetterRange = NarrowLetterRange(
			wordCriteria.LetterCriteria[1].Letter, wordCriteria.LetterCriteria[1].Status, letterRanges.Second);
		var thirdLetterRange = NarrowLetterRange(
			wordCriteria.LetterCriteria[2].Letter, wordCriteria.LetterCriteria[2].Status, letterRanges.Third);
		var fourthLetterRange = NarrowLetterRange(
			wordCriteria.LetterCriteria[3].Letter, wordCriteria.LetterCriteria[3].Status, letterRanges.Fourth);
		var fifthLetterRange = NarrowLetterRange(
			wordCriteria.LetterCriteria[4].Letter, wordCriteria.LetterCriteria[4].Status, letterRanges.Fifth);

		previousLetterRanges.Add(letterRanges);
		letterRanges = new LetterRanges(
			firstLetterRange,
			secondLetterRange,
			thirdLetterRange,
			fourthLetterRange,
			fifthLetterRange);
	}

	private LetterRange NarrowLetterRange(char letter, LetterStatus letterStatus, LetterRange existingLetterRange)
	{
		char startRange;
		char endRange;
		switch (letterStatus.Name)
		{
			case nameof(LetterStatus.Higher):
				startRange = letter >= existingLetterRange.StartRange ? (char)(letter + 1) : existingLetterRange.StartRange;
				endRange = existingLetterRange.EndRange;
				break;
			case nameof(LetterStatus.Lower):
				startRange = existingLetterRange.StartRange;
				endRange = letter <= existingLetterRange.EndRange ? (char)(letter - 1) : existingLetterRange.EndRange;
				break;
			case nameof(LetterStatus.Correct):
				startRange = letter;
				endRange = letter;
				break;
			case nameof(LetterStatus.Unknown):
			default:
				startRange = existingLetterRange.StartRange;
				endRange = existingLetterRange.EndRange;
				break;
		}

		return new LetterRange(startRange, endRange);
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

	public void UndoWordChoice()
	{
		if (chosenWords.Count == 0)
		{
			return;
		}

		chosenWords.RemoveAt(chosenWords.Count - 1);
		letterRanges = previousLetterRanges.Last();
		previousLetterRanges.RemoveAt(previousLetterRanges.Count - 1);
	}

	public void Reset()
	{
		chosenWords = new List<WordCriteria>();
		letterRanges = LetterRanges.Default;
		previousLetterRanges = new List<LetterRanges>();
	}

	public WordCriteria GetWordCriteriaForKnownAnswer(
		string answerWord, string chosenWord, LetterRanges currentLetterRanges)
	{
		var letterRanges = new List<LetterRange>();
		letterRanges.AddRange(new[]
		{
			currentLetterRanges.First,
			currentLetterRanges.Second,
			currentLetterRanges.Third,
			currentLetterRanges.Fourth,
			currentLetterRanges.Fifth
		});
		var narrowedLetterRanges = new List<LetterRange>();
		var letterStatuses = new List<LetterStatus>();

		// Iterate through each character and set LetterStatus and associated values to build LetterCriteria after
		for (var i = 0; i < chosenWord.Length; i++)
		{
			if (chosenWord[i] < answerWord[i])
			{
				narrowedLetterRanges.Add(NarrowLetterRange(chosenWord[i], LetterStatus.Higher, letterRanges[i]));
				letterStatuses.Add(LetterStatus.Higher);
			}
			else if (chosenWord[i] > answerWord[i])
			{
				narrowedLetterRanges.Add(NarrowLetterRange(chosenWord[i], LetterStatus.Lower, letterRanges[i]));
				letterStatuses.Add(LetterStatus.Lower);
			}
			else if (chosenWord[i] == answerWord[i])
			{
				narrowedLetterRanges.Add(NarrowLetterRange(chosenWord[i], LetterStatus.Correct, letterRanges[i]));
				letterStatuses.Add(LetterStatus.Correct);
			}
			else
			{
				narrowedLetterRanges.Add(NarrowLetterRange(chosenWord[i], LetterStatus.Unknown, letterRanges[i]));
				letterStatuses.Add(LetterStatus.Unknown);
			}
		}

		var letterCriteria = new List<LetterCriteria>
			{
				new LetterCriteria(chosenWord[0], letterStatuses[0], narrowedLetterRanges[0]),
				new LetterCriteria(chosenWord[1], letterStatuses[1], narrowedLetterRanges[1]),
				new LetterCriteria(chosenWord[2], letterStatuses[2], narrowedLetterRanges[2]),
				new LetterCriteria(chosenWord[3], letterStatuses[3], narrowedLetterRanges[3]),
				new LetterCriteria(chosenWord[4], letterStatuses[4], narrowedLetterRanges[4])
			};
		var wordCriteria = new WordCriteria(letterCriteria);

		return wordCriteria;
	}
}
