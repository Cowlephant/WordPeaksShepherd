namespace WordlePeaksShepherd.Services.Interfaces;

public interface ILetterService
{
	public bool IsValidLetter(char letter);

	public string GenerateLettersInRange(LetterRange letterRange);

	public int GetLetterScore(char letter, LetterRange letterRange);

	public int GetWordScore(IEnumerable<LetterCriteria> letterCriteria);
}
