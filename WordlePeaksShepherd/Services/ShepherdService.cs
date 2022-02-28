using System.Diagnostics.CodeAnalysis;

namespace WordlePeaksShepherd.Services;

public sealed class ShepherdService : IShepherdService
{
	private IWordService wordService;
	private ILetterService letterService;

	private string rawWords;
	public IEnumerable<string> Words => wordService.GetPotentialAnswerWords();

	public ShepherdService(IWordService wordService, ILetterService letterService)
	{
		this.wordService = wordService;
		this.letterService = letterService;
		InitializeWords();
	}

	[MemberNotNull(nameof(rawWords))]
	private void InitializeWords()
	{
		rawWords = string.Join(" ", wordService.GetPotentialAnswerWords());
	}

	public int GetLetterScore(char letter, char inclusiveStart, char inclusiveEnd)
	{
		var lowerLetter = Char.ToLower(letter);

		var rangeIsInvalid =
			!letterService.IsValidLetter(inclusiveStart) ||
			!letterService.IsValidLetter(inclusiveEnd);
		if (rangeIsInvalid)
		{
			throw new ShepherdException("Range must consist of valid English letters.");
		}
		var letterIsInvalid = !letterService.IsValidLetter(lowerLetter);
		if (letterIsInvalid)
		{
			throw new ShepherdException("Letter must be valid English character.");
		}

		var characterRange = letterService.GenerateLettersInRange(inclusiveStart, inclusiveEnd);
		int letterIndex = characterRange.IndexOf(lowerLetter);

		int lettersRemoveFromLeft = letterIndex;
		int lettersRemovedFromRight = (characterRange.Length - 1) - letterIndex;

		var letterScore = Math.Abs(lettersRemoveFromLeft - lettersRemovedFromRight);

		return letterScore;
	}

	public IEnumerable<string> GetWordChoices(ShepherdWordCriteria wordCriteria)
	{
		throw new NotImplementedException();
	}
}
