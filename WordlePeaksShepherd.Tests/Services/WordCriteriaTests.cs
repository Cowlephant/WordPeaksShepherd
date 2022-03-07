using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class WordCriteriaTests
{
	private List<LetterCriteria> validLetterCriteria;

	public WordCriteriaTests()
	{
		validLetterCriteria = new List<LetterCriteria>
		{
			new LetterCriteria('a', LetterStatus.Correct, new LetterRange('a', 'z')),
			new LetterCriteria('p', LetterStatus.Correct, new LetterRange('a', 'v')),
			new LetterCriteria('p', LetterStatus.Correct, new LetterRange('a', 'z')),
			new LetterCriteria('l', LetterStatus.Correct, new LetterRange('l', 'z')),
			new LetterCriteria('e', LetterStatus.Correct, new LetterRange('a', 'm'))
		};
	}

	[Fact]
	public void ShouldNotThrowExceptionWhenCreatedWithExactlyFiveLetterCriteria()
	{
		var letterCriteria = validLetterCriteria;
		var wordCriteria = new WordCriteria(letterCriteria);
		var expectedLetterCriteriaCount = 5;

		var letterCriteriaCount = wordCriteria.LetterCriteria.Count();

		Assert.Equal(expectedLetterCriteriaCount, letterCriteriaCount);
	}

	[Fact]
	public void ShouldThrowExceptionWhenCreatedWithFewerThanFiveLetterCriteria()
	{
		var letterCriteria = validLetterCriteria;
		letterCriteria.RemoveAt(letterCriteria.Count() - 1);

		var exception = Assert.Throws<ArgumentOutOfRangeException>(
			() => new WordCriteria(letterCriteria));
		var expectedMessage = "Expected 5 letters, received 4. (Parameter 'letterCriteria')";

		Assert.Equal(expectedMessage, exception.Message);
	}

	[Fact]
	public void ShouldThrowExceptionWhenCreatedWithMoreThanFiveLetterCriteria()
	{
		var letterCriteria = validLetterCriteria;
		letterCriteria.Add(new LetterCriteria('s', LetterStatus.Correct, new LetterRange('a', 's')));

		var exception = Assert.Throws<ArgumentOutOfRangeException>(
			() => new WordCriteria(letterCriteria));
		var expectedMessage = "Expected 5 letters, received 6. (Parameter 'letterCriteria')";

		Assert.Equal(expectedMessage, exception.Message);
	}

	[Fact]
	public void IsSolved_ShouldReturnTrueWhenAllLetterCriteriaHasCorrectLetterStatus()
	{
		var letterCriteria = validLetterCriteria;
		var wordCriteria = new WordCriteria(letterCriteria);

		var isSolved = wordCriteria.IsSolved;

		Assert.True(isSolved);
	}

	[Fact]
	public void IsSolved_ShouldReturnFalseWhenAnyLetterCriteriaHasOtherStatusThanCorrect()
	{
		var letterCriteria = validLetterCriteria;
		letterCriteria.RemoveAt(0);
		letterCriteria.Insert(0, new LetterCriteria('a', LetterStatus.Higher, new LetterRange('a', 'z')));

		var wordCriteria = new WordCriteria(letterCriteria);

		var isSolved = wordCriteria.IsSolved;

		Assert.False(isSolved);
	}
}
