namespace WordlePeaksShepherd.Services;

public sealed class WordlePeaksWordService : IWordService
{
	private List<string> words;

	public WordlePeaksWordService(Stream wordFileStream)
	{
		using var fileReader = new StreamReader(wordFileStream);
		var rawWords = fileReader.ReadToEnd()
			.ReplaceLineEndings("\n")
			.Replace('\n', ' ');
		words = rawWords.Split(" ").ToList();
		fileReader.Close();
		wordFileStream.Close();
	}

	public IEnumerable<string> GetPotentialAnswerWords()
	{
		return words.AsReadOnly();
	}
}
