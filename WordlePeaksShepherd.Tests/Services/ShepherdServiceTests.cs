using Microsoft.Extensions.DependencyInjection;
using WordlePeaksShepherd.Services;
using WordlePeaksShepherd.Services.Interfaces;
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
}
