using System.Text;

namespace WordlePeaksShepherd.Services;

public sealed class ShepherdLetterService : ILetterService
{
	public ShepherdLetterService()
	{
	}

	public bool IsValidLetter(char letter)
	{
		var lowerLetter = Char.ToLower(letter);
		var isValid = lowerLetter >= 'a' && lowerLetter <= 'z';
		return isValid;
	}

	public string GenerateLettersInRange(char inclusiveStart, char inclusiveEnd)
	{
		var lowerStart = Char.ToLower(inclusiveStart);
		var lowerEnd = Char.ToLower(inclusiveEnd);

		var valuesOutOfOrder = (int)lowerEnd < (int)lowerStart;
		if (valuesOutOfOrder)
		{
			(lowerStart, lowerEnd) = (lowerEnd, lowerStart);
		}

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
}
