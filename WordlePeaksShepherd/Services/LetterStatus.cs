using Ardalis.SmartEnum;

namespace WordlePeaksShepherd.Services;

public sealed class LetterStatus : SmartEnum<LetterStatus>
{
	public static LetterStatus Correct = new LetterStatus("Correct", 1);
	public static LetterStatus Higher = new LetterStatus("Higher", 2);
	public static LetterStatus Lower = new LetterStatus("Lower", 3);
	public static LetterStatus Outside = new LetterStatus("Outside", 4);

	public LetterStatus(string name, int value) : base(name, value)
	{
	}
}
