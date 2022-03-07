using WordlePeaksShepherd.Services.Interfaces;

namespace WordlePeaksShepherd.Services;

public sealed class WordService : IWordService
{
	private List<string> words;

	public WordService(Stream wordFileStream)
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
