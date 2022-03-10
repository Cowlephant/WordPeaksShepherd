using Ardalis.SmartEnum;

namespace WordlePeaksShepherd.Services;

//ncrunch: no coverage start
public sealed class LetterStatus : SmartEnum<LetterStatus>
{
	public static LetterStatus Unknown = new LetterStatus("Unknown", 1);
	public static LetterStatus Correct = new LetterStatus("Correct", 2);
	public static LetterStatus Higher = new LetterStatus("Higher", 3);
	public static LetterStatus Lower = new LetterStatus("Lower", 4);

	public LetterStatus(string name, int value) : base(name, value)
	{
	}
}
//ncrunch: no coverage end
