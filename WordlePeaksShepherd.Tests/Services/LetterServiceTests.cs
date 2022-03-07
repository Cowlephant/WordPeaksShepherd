using System.Collections;
using WordlePeaksShepherd.Exceptions;
using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class LetterServiceTests
{
	private LetterService service;

	public LetterServiceTests()
	{
		service = new LetterService();
	}

	[Theory]
	[InlineData('a')]
	[InlineData('A')]
	[InlineData('z')]
	[InlineData('Z')]
	public void IsValidLetter_ShouldReturnTrueForValidEnglishLetters(char letter)
	{
		var isValidLetter = service.IsValidLetter(letter);

		Assert.True(isValidLetter);
	}

	[Theory]
	[InlineData('0')]
	[InlineData('9')]
	[InlineData('!')]
	[InlineData(' ')]
	[InlineData('\t')]
	[InlineData('á')]
	[InlineData('é')]
	[InlineData('ᴃ')]
	[InlineData('ꜰ')]
	public void IsValidLetter_ShouldReturnFalseForNonEnglishLetters(char letter)
	{
		var isValidLetter = service.IsValidLetter(letter);

		Assert.False(isValidLetter);
	}

	[Theory]
	[InlineData('a', 'z', "abcdefghijklmnopqrstuvwxyz")]
	[InlineData('c', 'p', "cdefghijklmnop")]
	public void GenerateLettersInRange_ShouldReturnExpectedRangeGivenValidValues(
		char startRange, char endRange, string expectedCharacters)
	{
		var characterRange = service.GenerateLettersInRange(new LetterRange(startRange, endRange));

		Assert.Equal(expectedCharacters, characterRange);
	}

	[Fact]
	public void GenerateLettersInRange_ShouldReturnExpectedRangeWithValidValuesOutOfOrder()
	{
		char startRange = 'z';
		char endRange = 'a';
		var expectedCharacters = "abcdefghijklmnopqrstuvwxyz";

		var characterRange = service.GenerateLettersInRange(new LetterRange(startRange, endRange));

		Assert.Equal(expectedCharacters, characterRange);
	}

	[Theory]
	[InlineData('l', 'a', 'z', 3)] // 11 eliminated from left, 14 from right for score of [3]
	[InlineData('m', 'a', 'z', 1)] // 12 eliminated from left, 13 from right for score of [1]
	[InlineData('a', 'a', 'z', 25)] // 0 eliminated from left, 25 from right for score of [25]
	[InlineData('n', 'e', 'r', 5)] // 9 eliminated from left, 4 from right for score of [5]
	[InlineData('b', 'a', 'c', 0)] // 1 eliminated from left, 1 from right for score of [0]
	[InlineData('m', 'm', 'n', 1)] // 0 eliminated from left, 1 from right for score of [1]
	[InlineData('T', 'T', 'T', 0)] // 0 eliminated from left, 0 from right for score of [0]
	public void GetLetterScore_ShouldReturnExpectedValuesForGivenInputs(
		char letter, char startRange, char endRange, int expectedScore)
	{
		var letterScore = service.GetLetterScore(letter, new LetterRange(startRange, endRange));

		Assert.Equal(expectedScore, letterScore);
	}

	[Fact]
	public void GetLetterScore_ShouldThrowExceptionGivenInvalidLetter()
	{
		char invalidLetter = '5';
		char invalidStartRange = 'a';
		char validEndRange = 'z';

		var exception = Assert.Throws<ShepherdException>(
			() => service.GetLetterScore(invalidLetter, new LetterRange(invalidStartRange, validEndRange)));
		var expectedMessage = "Letter must be valid English character.";

		Assert.Equal(expectedMessage, exception.Message);
	}

	[Fact]
	public void GetLetterScore_ShouldThrowExceptionGivenInvalidRange()
	{
		char validLetter = 'm';
		char invalidStartRange = 'á';
		char validEndRange = 'z';

		var exception = Assert.Throws<ShepherdException>(
			() => service.GetLetterScore(validLetter, new LetterRange(invalidStartRange, validEndRange)));
		var expectedMessage = "Range must consist of valid English letters.";

		Assert.Equal(expectedMessage, exception.Message);
	}

	[Theory, ClassData(typeof(SampleWordCriteriaData))]
	public void GetWordScore_ShouldReturnExpectedValueForGivenWordCriteria(
		WordCriteria wordCriteria, int expectedScore)
	{
		var score = service.GetWordScore(wordCriteria.LetterCriteria);

		Assert.Equal(expectedScore, score);
	}

	public class SampleWordCriteriaData : IEnumerable<object[]>
	{
		private readonly List<object[]> data = new List<object[]>
		{
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('a', LetterStatus.Higher, new LetterRange('a', 'z')),
					new LetterCriteria('p', LetterStatus.Lower, new LetterRange('a', 'z')),
					new LetterCriteria('p', LetterStatus.Higher, new LetterRange('a', 'z')),
					new LetterCriteria('l', LetterStatus.Correct, new LetterRange('a', 'z')),
					new LetterCriteria('e', LetterStatus.Lower, new LetterRange('a', 'z'))
				}), 55
			},
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('g', LetterStatus.Higher, new LetterRange('d', 'm')),
					new LetterCriteria('r', LetterStatus.Higher, new LetterRange('m', 'z')),
					new LetterCriteria('a', LetterStatus.Higher, new LetterRange('a', 'm')),
					new LetterCriteria('n', LetterStatus.Lower, new LetterRange('c', 'n')),
					new LetterCriteria('t', LetterStatus.Lower, new LetterRange('f', 'v'))
				}), 41
			},
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('p', LetterStatus.Correct, new LetterRange('p', 'p')),
					new LetterCriteria('e', LetterStatus.Correct, new LetterRange('e', 'e')),
					new LetterCriteria('a', LetterStatus.Correct, new LetterRange('a', 'a')),
					new LetterCriteria('c', LetterStatus.Higher, new LetterRange('c', 'p')),
					new LetterCriteria('e', LetterStatus.Lower, new LetterRange('d', 'e'))
				}), 14
			},
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('o', LetterStatus.Higher, new LetterRange('a', 'z')),
					new LetterCriteria('n', LetterStatus.Lower, new LetterRange('a', 'z')),
					new LetterCriteria('i', LetterStatus.Higher, new LetterRange('a', 'z')),
					new LetterCriteria('o', LetterStatus.Correct, new LetterRange('a', 'z')),
					new LetterCriteria('n', LetterStatus.Lower, new LetterRange('a', 'z'))
				}), 17
			},
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('w', LetterStatus.Outside, new LetterRange('a', 'm')),
					new LetterCriteria('r', LetterStatus.Outside, new LetterRange('a', 'm')),
					new LetterCriteria('o', LetterStatus.Outside, new LetterRange('a', 'm')),
					new LetterCriteria('n', LetterStatus.Outside, new LetterRange('a', 'm')),
					new LetterCriteria('g', LetterStatus.Outside, new LetterRange('a', 'e'))
				}), 57
			},
			new object[]
			{
				new WordCriteria(new List<LetterCriteria>
				{
					new LetterCriteria('w', LetterStatus.Higher, new LetterRange('g', 'z')),
					new LetterCriteria('e', LetterStatus.Lower, new LetterRange('a', 'm')),
					new LetterCriteria('i', LetterStatus.Correct, new LetterRange('f', 's')),
					new LetterCriteria('r', LetterStatus.Outside, new LetterRange('a', 'm')),
					new LetterCriteria('d', LetterStatus.Higher, new LetterRange('a', 'z'))
				}), 56
			}
		};

		public IEnumerator<object[]> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}