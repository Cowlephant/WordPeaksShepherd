namespace WordlePeaksShepherd.Services;

public interface IShepherdService
{
	public IEnumerable<string> Words { get; }

	public int GetLetterScore(char letter, char inclusiveStart, char inclusiveEnd);

	public IEnumerable<string> GetWordChoices(ShepherdWordCriteria wordCriteria);
}
