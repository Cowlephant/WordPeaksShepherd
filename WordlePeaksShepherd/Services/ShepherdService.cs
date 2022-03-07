using System.Diagnostics.CodeAnalysis;
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

	public ShepherdService(IWordService wordService, ILetterService letterService)
	{
		this.wordService = wordService;
		this.letterService = letterService;

		InitializeWords();
		Reset();
	}

	[MemberNotNull(nameof(rawWords))]
	private void InitializeWords()
	{
		rawWords = string.Join(" ", wordService.GetPotentialAnswerWords());
	}

	public IEnumerable<Word> AddWordChoice(WordCriteria wordCriteria)
	{
		chosenWords.Add(wordCriteria);

		letterService.GetWordScore(wordCriteria.LetterCriteria);

		return GetSuggestedWords();
	}

	public IEnumerable<Word> GetSuggestedWords()
	{
		throw new NotImplementedException();
	}

	public void UndoLastWordChoice()
	{
		if(chosenWords.Count == 0)
		{
			return;
		}

		chosenWords.RemoveAt(chosenWords.Count - 1);
	}

	[MemberNotNull(nameof(chosenWords))]
	[MemberNotNull(nameof(letterRanges))]
	public void Reset()
	{
		chosenWords = new List<WordCriteria>();
		letterRanges = LetterRanges.Default;
	}
}
