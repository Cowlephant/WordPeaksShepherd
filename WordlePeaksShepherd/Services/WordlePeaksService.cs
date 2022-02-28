using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace WordlePeaksShepherd.Services;

public sealed class WordlePeaksService
{
	private string rawWords;
	public IEnumerable<string> Words => rawWords.Split(' ');

	public WordlePeaksService()
	{
		InitializeWords();
	}

	[MemberNotNull(nameof(rawWords))]
	private void InitializeWords()
	{
		var wordsResourceStream = typeof(Program).Assembly.GetManifestResourceStream(
			"WordlePeaksShepherd.Data.words-potential.txt")!;
		using var fileReader = new StreamReader(wordsResourceStream);
		rawWords = fileReader.ReadToEnd()
			.ReplaceLineEndings("\n");
		fileReader.Close();
	}

	public int GetLetterScore(char letter, char inclusiveStart, char inclusiveEnd)
	{
		var lowerLetter = Char.ToLower(letter);

		var rangeIsInvalid = !IsValidLetter(inclusiveStart) || !IsValidLetter(inclusiveEnd);
		if (rangeIsInvalid)
		{
			throw new WordleException("Range must consist of valid English letters.");
		}
		var letterIsInvalid = !IsValidLetter(lowerLetter);
		if (letterIsInvalid)
		{
			throw new WordleException("Letter must be valid English character.");
		}

		var characterRange = GenerateCharactersInRange(inclusiveStart, inclusiveEnd);
		int letterIndex = characterRange.IndexOf(lowerLetter);

		int lettersRemoveFromLeft = letterIndex;
		int lettersRemovedFromRight = (characterRange.Length - 1) - letterIndex;

		var letterScore = Math.Abs(lettersRemoveFromLeft - lettersRemovedFromRight);

		return letterScore;
	}

	public bool IsValidLetter(char letter)
	{
		var lowerLetter = Char.ToLower(letter);
		var isValid = lowerLetter >= 'a' && lowerLetter <= 'z';
		return isValid;
	}

	public string GenerateCharactersInRange(char inclusiveStart, char inclusiveEnd)
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