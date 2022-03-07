using WordlePeaksShepherd.Exceptions;

namespace WordlePeaksShepherd.Services;

public sealed class LetterCriteria : ValueObject
{
	public char Letter { get; init; }
	public LetterStatus Status { get; private set; }
	
	[IgnoreMember]
	public LetterRange LetterRange { get; private set; }
	[IgnoreMember]
	public bool IsCorrect { get; private set; }

	public LetterCriteria(char letter, LetterStatus letterStatus, LetterRange letterRange)
	{
		Letter = letter;
		Status = letterStatus;
		LetterRange = letterRange;

		ValidateLetterStatus();
	}

	private void ValidateLetterStatus()
	{
		var letterNotInRange = Letter < LetterRange.StartRange || Letter > LetterRange.EndRange;
		if (letterNotInRange && Status != LetterStatus.Outside)
		{
			Status = LetterStatus.Outside;
		}

		if (Status == LetterStatus.Correct)
		{
			IsCorrect = true;
		}
	}
}
