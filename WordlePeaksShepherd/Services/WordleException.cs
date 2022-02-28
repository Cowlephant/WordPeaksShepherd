namespace WordlePeaksShepherd.Services;

public sealed class WordleException : Exception
{
	public WordleException(string message) : base(message)
	{
	}

	public WordleException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
