namespace WordlePeaksShepherd.Services;

public interface IWordService
{
	public IEnumerable<string> GetPotentialAnswerWords();
}
