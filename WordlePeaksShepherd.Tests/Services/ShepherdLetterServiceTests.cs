using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class ShepherdLetterServiceTests
{
	private ShepherdLetterService service;

	public ShepherdLetterServiceTests()
	{
		service = new ShepherdLetterService();
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
		var characterRange = service.GenerateLettersInRange(startRange, endRange);

		Assert.Equal(expectedCharacters, characterRange);
	}

	[Fact]
	public void GenerateLettersInRange_ShouldReturnExpectedRangeWithValidValuesOutOfOrder()
	{
		char startRange = 'z';
		char endRange = 'a';
		var expectedCharacters = "abcdefghijklmnopqrstuvwxyz";

		var characterRange = service.GenerateLettersInRange(startRange, endRange);

		Assert.Equal(expectedCharacters, characterRange);
	}
}
