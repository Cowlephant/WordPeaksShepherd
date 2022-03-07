namespace WordlePeaksShepherd.Services;

public class LetterRanges
{
	public LetterRange First { get; init; }
	public LetterRange Second { get; init; }
	public LetterRange Third { get; init; }
	public LetterRange Fourth { get; init; }
	public LetterRange Fifth { get; init; }

	public LetterRanges(
		LetterRange first,
		LetterRange second,
		LetterRange third,
		LetterRange fourth,
		LetterRange fifth)
	{
		First = first;
		Second = second;
		Third = third;
		Fourth = fourth;
		Fifth = fifth;
	}

	public static LetterRanges Default = new LetterRanges(
		LetterRange.Default,
		LetterRange.Default,
		LetterRange.Default,
		LetterRange.Default,
		LetterRange.Default);
}
