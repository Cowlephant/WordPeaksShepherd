using System.Text;
using WordlePeaksShepherd.Exceptions;
using WordlePeaksShepherd.Services.Interfaces;

namespace WordlePeaksShepherd.Services;

public sealed class LetterService : ILetterService
{
	public LetterService()
	{
	}

	public bool IsValidLetter(char letter)
	{
		var lowerLetter = Char.ToLower(letter);
		var isValid = lowerLetter >= 'a' && lowerLetter <= 'z';
		return isValid;
	}

	public string GenerateLettersInRange(LetterRange letterRange)
	{
		var lowerStart = Char.ToLower(letterRange.StartRange);
		var lowerEnd = Char.ToLower(letterRange.EndRange);

		var rangeStart = (int)lowerStart;
		var rangeCount = ((int)lowerEnd) - rangeStart + 1;
		var indexRange = Enumerable.Range(rangeStart, rangeCount);

		var characterStringBuilder = new StringBuilder(indexRange.Count());

		foreach (var characterIndex in indexRange)
		{
			characterStringBuilder.Append((char)characterIndex);
		}

		return characterStringBuilder.ToString();
	}

	public int GetLetterScore(char letter, LetterRange letterRange)
	{
		var lowerLetter = Char.ToLower(letter);

		var rangeIsInvalid =
			!IsValidLetter(letterRange.StartRange) ||
			!IsValidLetter(letterRange.EndRange);
		if (rangeIsInvalid)
		{
			throw new ShepherdException("Range must consist of valid English letters.");
		}
		var letterIsInvalid = !IsValidLetter(lowerLetter);
		if (letterIsInvalid)
		{
			throw new ShepherdException("Letter must be valid English character.");
		}

		var characterRange = GenerateLettersInRange(letterRange);
		int letterIndex = characterRange.IndexOf(lowerLetter);

		var letterOutsideRange = letterIndex == -1;
		if (letterOutsideRange)
		{
			return characterRange.Length;
		}

		int lettersRemoveFromLeft = letterIndex;
		int lettersRemovedFromRight = (characterRange.Length - 1) - letterIndex;

		var letterScore = Math.Abs(lettersRemoveFromLeft - lettersRemovedFromRight);

		return letterScore;
	}

	public int GetWordScore(IEnumerable<LetterCriteria> letterCriteria)
	{
		int combinedScore = 0;

		foreach (var criteria in letterCriteria)
		{
			combinedScore += GetLetterScore(criteria.Letter, criteria.LetterRange);
		}

		return combinedScore;
	}
}
