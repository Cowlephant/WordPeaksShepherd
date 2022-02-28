using Microsoft.Extensions.DependencyInjection;
using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class ShepherdServiceTests : IClassFixture<ContainerFixture>
{
	private ShepherdService service;

	public ShepherdServiceTests(ContainerFixture fixture)
	{
		var wordService = fixture.ServiceProvider.GetService<IWordService>()!;
		var letterService = fixture.ServiceProvider.GetService<ILetterService>()!;
		service = new ShepherdService(wordService, letterService);
	}

	[Fact]
	public void Words_ShouldReturnAllExpectedWords()
	{
		var expectedWords = new List<string> {
			"hello", "quite", "right", "sadly", "about",
			"three", "weeks", "makes", "basic", "tasks",
			"quite", "tough", "never", "smart", "enjoy",
			"other", "games", "learn", "extra", "words",
			"heard", "worse", "ideas", "great", "adieu" };

		var words = service.Words;

		Assert.Equal(expectedWords, words);
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

		var exception = Assert.Throws<ShepherdException>(
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

		var exception = Assert.Throws<ShepherdException>(
			() => service.GetLetterScore(validLetter, invalidStartRange, validEndRange));
		var expectedMessage = "Range must consist of valid English letters.";

		Assert.Equal(expectedMessage, exception.Message);
	}
}
