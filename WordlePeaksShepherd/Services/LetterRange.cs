namespace WordlePeaksShepherd.Services;

public class LetterRange : ValueObject
{
	public char StartRange { get; init; }
	public char EndRange { get; init; }

	public LetterRange(char inclusiveStartRange, char inclusiveEndRange)
	{
		StartRange = inclusiveStartRange;
		EndRange = inclusiveEndRange;

		if (StartRange > EndRange)
		{
			StartRange = EndRange;
			EndRange = inclusiveStartRange;
		}
	}

	public static LetterRange Default = new LetterRange('a', 'z');
}
