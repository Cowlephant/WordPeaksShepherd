using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class LetterCriteriaTests
{
	[Fact]
	public void Status_WillBeChangedToOutsideWhenProvidedLetterIsOutsideProvidedRange()
	{
		var letterRange = new LetterRange('g', 'm');
		var letterCriteria = new LetterCriteria('a', LetterStatus.Higher, letterRange);
		var expected = LetterStatus.Outside;

		var status = letterCriteria.Status;

		Assert.Equal(expected, status);
	}
}
