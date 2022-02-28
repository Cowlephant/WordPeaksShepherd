using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class WordlePeaksServiceTests
{
	private WordlePeaksService service;

	public WordlePeaksServiceTests()
	{
		service = new WordlePeaksService();
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
	public void GenerateCharactersInRange_ShouldReturnExpectedRangeGivenValidValues(
		char startRange, char endRange, string expectedCharacters)
	{
		var characterRange = service.GenerateCharactersInRange(startRange, endRange);

		Assert.Equal(expectedCharacters, characterRange);
	}

	[Fact]
	public void GenerateCharactersInRange_ShouldReturnExpectedRangeWithValidValuesOutOfOrder()
	{
		char startRange = 'z';
		char endRange = 'a';
		var expectedCharacters = "abcdefghijklmnopqrstuvwxyz";

		var characterRange = service.GenerateCharactersInRange(startRange, endRange);

		Assert.Equal(expectedCharacters, characterRange);
	}

	[Theory]
	[InlineData('l', 'a', 'z', 3)] // 11 eliminated from left, 14 from right for score of [3]
	[InlineData('m', 'a', 'z', 1)] // 12 eliminated from left, 13 from right for score of [1]
	[InlineData('n', 'e', 'r', 5)] // 9 eliminated from left, 4 from right for score of [5]
	[InlineData('b', 'a', 'c', 0)] // 1 eliminated from left, 1 from right for score of [0]
	[InlineData('m', 'm', 'n', 1)] // 0 eliminated from left, 1 from right for score of [1]
	[InlineData('T', 'T', 'T', 0)] // 0 eliminated from left, 0 from right for score of [0]
	public void GetLetterScore_ShouldReturnExpectedValuesForGivenInputs(
		char letter, char startRange, char endRange, int expectedScore)
	{
		var letterScore = service.GetLetterScore(letter, startRange, endRange);

		Assert.Equal(expectedScore, letterScore);
	}

	[Fact]
	public void GetLetterScore_ShouldThrowExceptionGivenInvalidLetter()
	{
		char invalidLetter = '5';
		char invalidStartRange = 'a';
		char validEndRange = 'z';

		var exception = Assert.Throws<WordleException>(
			() => service.GetLetterScore(invalidLetter, invalidStartRange, validEndRange));
		var expectedMessage = "Letter must be valid English character.";

		Assert.Equal(expectedMessage, exception.Message);
	}

	[Fact]
	public void GetLetterScore_ShouldThrowExceptionGivenInvalidRange()
	{
		char validLetter = 'm';
		char invalidStartRange = 'á';
		char validEndRange = 'z';

		var exception = Assert.Throws<WordleException>(
			() => service.GetLetterScore(validLetter, invalidStartRange, validEndRange));
		var expectedMessage = "Range must consist of valid English letters.";

		Assert.Equal(expectedMessage, exception.Message);
	}
}
