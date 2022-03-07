using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections;
using WordlePeaksShepherd.Services;
using WordlePeaksShepherd.Services.Interfaces;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class ShepherdServiceTests : IClassFixture<ContainerFixture>
{
	private ContainerFixture fixture;
	private IWordService wordService;
	private ILetterService letterService;

	public ShepherdServiceTests(ContainerFixture fixture)
	{
		this.fixture = fixture;
		wordService = fixture.ServiceProvider.GetService<IWordService>()!;
		letterService = fixture.ServiceProvider.GetService<ILetterService>()!;
	}

	[Fact]
	public void Words_ShouldReturnAllExpectedWords()
	{
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var expectedWords = File.ReadAllText("Data/test-potential-words.txt").Split("\r\n");

		var words = service.Words;

		Assert.Equal(expectedWords, words);
	}

	[Fact]
	public void GetSuggestedWords_ShouldReturnAllWordsWhenLetterRangesIsFullAlphabet()
	{
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var expectedWords = File.ReadAllText("Data/test-potential-words.txt").Split("\r\n");

		var words = service.GetSuggestedWords();

		Assert.Equal(expectedWords, words);
	}

	[Theory, ClassData(typeof(LetterRangesExpectedWords))]
	public void GetSuggestedWords_ShouldReturnExpectedWordsGivenLetterRanges(
		LetterRanges letterRanges, IEnumerable<string> expectedWords)
	{
		var service = new ShepherdService(letterRanges, wordService, letterService);

		var words = service.GetSuggestedWords();

		Assert.Equal(expectedWords, words);
	}

	public class LetterRangesExpectedWords : IEnumerable<object[]>
	{
		private readonly List<object[]> data = new List<object[]>
		{
			new object[]
			{
				new LetterRanges(
					new LetterRange('f', 'r'),
					new LetterRange('f', 'r'),
					new LetterRange('f', 'r'),
					new LetterRange('f', 'r'),
					new LetterRange('f', 'r')),
				new string[] { "hippo", "igloo", "grill" }
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't')),
				new string[] {
					"skill", "shirt", "timer", "hippo", "igloo",
					"slide", "stole", "spoon", "melon", "phone",
					"rigid", "grill", "steel", "their",	"trend", 
					"depth", "kneel", "loose", "snide", "tress", 
					"fight", "spite", "trope", "intro", "retro", 
					"spine", "piper", "smoke", "ridge", "oriel", 
					"spore", "shore", "ditto"
				}
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('d', 'm'),
					new LetterRange('e', 't'),
					new LetterRange('g', 'l'),
					new LetterRange('d', 'z'),
					new LetterRange('f', 't')),
				new string[] { "igloo", "melon", "grill", "fight" }
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm')),
				new string[] { "decal", "bleed", "magic", "medic" }
			},
						new object[]
			{
				new LetterRanges(
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z')),
				new string[] { "rusty", "sunny", "spoon", "roomy", "worst", "mourn" }
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g')),
				new string[] { }
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
