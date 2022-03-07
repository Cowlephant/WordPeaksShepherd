using Microsoft.Extensions.DependencyInjection;
using WordlePeaksShepherd.Services.Interfaces;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class WordServiceTests : IClassFixture<ContainerFixture>
{
	private ContainerFixture fixture;

	public WordServiceTests(ContainerFixture fixture)
	{
		this.fixture = fixture;
	}

	[Fact]
	public void GetPotentialAnswerWords_ShouldReturnAllExpectedWords()
	{
		var service = fixture.ServiceProvider.GetService<IWordService>()!;
		var expectedWords = File.ReadAllText("Data/test-potential-words.txt").Split("\r\n");

		var words = service.GetPotentialAnswerWords();

		Assert.Equal(expectedWords, words);
	}
}
