using WordlePeaksShepherd.Services;
using Xunit;

namespace WordlePeaksShepherd.Tests.Services;

public sealed class LetterRangeTests
{
	[Fact]
	public void RangesShouldBeInOrderWhenProvidedInOrder()
	{
		var letterRange = new LetterRange('a', 'z');

		Assert.Equal('a', letterRange.StartRange);
		Assert.Equal('z', letterRange.EndRange);
	}

	[Fact]
	public void RangesShouldBeSwappedOrderWhenProvidedOutOfOrder()
	{
		var letterRange = new LetterRange('z', 'a');

		Assert.Equal('a', letterRange.StartRange);
		Assert.Equal('z', letterRange.EndRange);
	}
}
