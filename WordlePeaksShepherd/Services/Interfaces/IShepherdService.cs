namespace WordlePeaksShepherd.Services.Interfaces;

public interface IShepherdService
{
	public IEnumerable<WordCriteria> ChosenWords { get; }
	public LetterRanges LetterRanges { get; }

	public void UndoLastWordChoice();
	public void Reset();
	public IEnumerable<Word> AddWordChoice(WordCriteria wordCriteria);

	public IEnumerable<Word> GetSuggestedWords();
}
