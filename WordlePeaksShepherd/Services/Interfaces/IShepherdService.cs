namespace WordlePeaksShepherd.Services.Interfaces;

public interface IShepherdService
{
	public IEnumerable<WordCriteria> ChosenWords { get; }
	public LetterRanges LetterRanges { get; }

	public void AddWordChoice(WordCriteria wordCriteria);
	public void UndoWordChoice();
	public void Reset();

	public IEnumerable<Word> GetSuggestedWords();
}
