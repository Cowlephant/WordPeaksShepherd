using FluentAssertions;
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

		// We're going to ignore the scores
		var words = service.GetSuggestedWords().Select(w => w.Value);

		Assert.Equal(expectedWords, words);
	}

	[Fact]
	public void AddWordChoice_ShouldAddChosenWordAndUpdateLetterRanges()
	{
		// Arrange
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var firstWordChoice = new WordCriteria(
			new List<LetterCriteria>
			{
						new LetterCriteria('a', LetterStatus.Higher, service.LetterRanges.First),
						new LetterCriteria('p', LetterStatus.Lower, service.LetterRanges.Second),
						new LetterCriteria('p', LetterStatus.Lower, service.LetterRanges.Third),
						new LetterCriteria('l', LetterStatus.Lower, service.LetterRanges.Fourth),
						new LetterCriteria('e', LetterStatus.Correct, service.LetterRanges.Fifth)
			});
		var secondWordChoice = new WordCriteria(
			new List<LetterCriteria>
			{
						new LetterCriteria('p', LetterStatus.Lower, service.LetterRanges.First),
						new LetterCriteria('e', LetterStatus.Higher, service.LetterRanges.Second),
						new LetterCriteria('a', LetterStatus.Higher, service.LetterRanges.Third),
						new LetterCriteria('c', LetterStatus.Higher, service.LetterRanges.Fourth),
						new LetterCriteria('e', LetterStatus.Correct, service.LetterRanges.Fifth)
			});
		var expectedChosenWords = new List<WordCriteria> { firstWordChoice, secondWordChoice };
		var expectedLetterRanges = new LetterRanges(
			new LetterRange('b', 'o'),
			new LetterRange('f', 'o'),
			new LetterRange('b', 'o'),
			new LetterRange('d', 'k'),
			new LetterRange('e', 'e'));

		service.AddWordChoice(firstWordChoice);
		service.AddWordChoice(secondWordChoice);

		Assert.Equal(expectedChosenWords, service.ChosenWords);
		service.LetterRanges.Should().BeEquivalentTo(expectedLetterRanges);
	}

	[Fact]
	public void UndoWordChoice_ShouldRemoveLastAddedChosenWordAndRestorePreviousLetterRanges()
	{
		// Arrange
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var firstWordCriteria = new WordCriteria(
			new List<LetterCriteria>
			{
						new LetterCriteria('a', LetterStatus.Higher, service.LetterRanges.First),
						new LetterCriteria('p', LetterStatus.Lower, service.LetterRanges.Second),
						new LetterCriteria('p', LetterStatus.Lower, service.LetterRanges.Third),
						new LetterCriteria('l', LetterStatus.Lower, service.LetterRanges.Fourth),
						new LetterCriteria('e', LetterStatus.Correct, service.LetterRanges.Fifth)
			});
		var secondWordCriteria = new WordCriteria(
			new List<LetterCriteria>
			{
						new LetterCriteria('d', LetterStatus.Correct, service.LetterRanges.First),
						new LetterCriteria('a', LetterStatus.Correct, service.LetterRanges.Second),
						new LetterCriteria('n', LetterStatus.Correct, service.LetterRanges.Third),
						new LetterCriteria('c', LetterStatus.Correct, service.LetterRanges.Fourth),
						new LetterCriteria('e', LetterStatus.Correct, service.LetterRanges.Fifth)
			});
		service.AddWordChoice(firstWordCriteria);
		service.AddWordChoice(secondWordCriteria);
		var expectedChosenWords = new List<WordCriteria> { firstWordCriteria };
		var expectedLetterRanges = new LetterRanges(
			new LetterRange('b', 'z'),
			new LetterRange('a', 'o'),
			new LetterRange('a', 'o'),
			new LetterRange('a', 'k'),
			new LetterRange('e', 'e'));

		// Act
		service.UndoWordChoice();

		Assert.Equal(expectedChosenWords, service.ChosenWords);
		service.LetterRanges.Should().BeEquivalentTo(expectedLetterRanges);
	}

	[Fact]
	public void UndoLastWordChoice_ShouldDoNothingWhenNoChosenWordsExist()
	{
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var expectedChosenWords = new List<WordCriteria>();
		var expectedLetterRanges = LetterRanges.Default;

		service.UndoWordChoice();

		Assert.Equal(expectedChosenWords, service.ChosenWords);
		Assert.Equal(expectedLetterRanges, service.LetterRanges);
	}

	[Fact]
	public void Reset_ShouldResetChosenWordsAndLetterRanges()
	{
		var service = new ShepherdService(LetterRanges.Default, wordService, letterService);
		var newWordCriteria = new WordCriteria(
			new List<LetterCriteria>
			{
				new LetterCriteria('a', LetterStatus.Higher, new LetterRange('a', 'z')),
				new LetterCriteria('p', LetterStatus.Higher, new LetterRange('a', 'z')),
				new LetterCriteria('p', LetterStatus.Higher, new LetterRange('a', 'z')),
				new LetterCriteria('l', LetterStatus.Higher, new LetterRange('a', 'z')),
				new LetterCriteria('e', LetterStatus.Higher, new LetterRange('a', 'z'))
			});
		service.AddWordChoice(newWordCriteria);

		var expectedChosenWords = new List<WordCriteria>();
		var expectedLetterRanges = LetterRanges.Default;

		service.Reset();

		Assert.Equal(expectedChosenWords, service.ChosenWords);
		Assert.Equal(expectedLetterRanges, service.LetterRanges);
	}

	[Theory, ClassData(typeof(LetterRangesExpectedWordsWithScores))]
	public void GetSuggestedWords_ShouldReturnExpectedWordsWithScoresGivenLetterRanges(
		LetterRanges letterRanges, IEnumerable<Word> expectedWords)
	{
		var service = new ShepherdService(letterRanges, wordService, letterService);

		var words = service.GetSuggestedWords();

		Assert.Equal(expectedWords, words);
	}

	//ncrunch: no coverage start
	public class LetterRangesExpectedWordsWithScores : IEnumerable<object[]>
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
				new Word[]
				{
					new Word("hippo", 36),
					new Word("igloo", 28),
					new Word("grill", 28)
				}
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't'),
					new LetterRange('d', 't')),
				new Word[] {
					new Word("skill", 22),
					new Word("shirt", 56),
					new Word("timer", 50),
					new Word("hippo", 36),
					new Word("igloo", 28),
					new Word("slide", 50),
					new Word("stole", 50),
					new Word("spoon", 38),
					new Word("melon", 26),
					new Word("phone", 40),
					new Word("rigid", 50),
					new Word("grill", 28),
					new Word("steel", 58),
					new Word("their", 56),
					new Word("trend", 62),
					new Word("depth", 62),
					new Word("kneel", 34),
					new Word("loose", 40),
					new Word("snide", 54),
					new Word("tress", 70),
					new Word("fight", 52),
					new Word("spite", 58),
					new Word("trope", 56),
					new Word("intro", 44),
					new Word("retro", 60),
					new Word("spine", 46),
					new Word("piper", 48),
					new Word("smoke", 38),
					new Word("ridge", 58),
					new Word("oriel", 38),
					new Word("spore", 54),
					new Word("shore", 54),
					new Word("ditto", 60)
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
				new Word[]
				{
					new Word("igloo", 21),
					new Word("melon", 31),
					new Word("grill", 23),
					new Word("fight", 45)
				}
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm'),
					new LetterRange('a', 'm')),
				new Word[]
				{
					new Word("decal", 40),
					new Word("bleed", 34),
					new Word("magic", 36),
					new Word("medic", 34)
				}
			},
						new object[]
			{
				new LetterRanges(
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z'),
					new LetterRange('m', 'z')),
				new Word[]
				{
					new Word("rusty", 19),
					new Word("sunny", 37),
					new Word("spoon", 37),
					new Word("roomy", 45),
					new Word("worst", 21),
					new Word("mourn", 39)
				}
			},
			new object[]
			{
				new LetterRanges(
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g'),
					new LetterRange('a', 'g')),
				new Word[] { }
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
	//ncrunch: no coverage end
}
