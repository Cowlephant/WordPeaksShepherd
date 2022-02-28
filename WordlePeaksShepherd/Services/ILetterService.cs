namespace WordlePeaksShepherd.Services;

public interface ILetterService
{
	public bool IsValidLetter(char letter);
	public string GenerateLettersInRange(char inclusiveStart, char inclusiveEnd);
}
