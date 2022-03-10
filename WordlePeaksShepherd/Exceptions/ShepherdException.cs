namespace WordlePeaksShepherd.Exceptions;

//ncrunch: no coverage start
public sealed class ShepherdException : Exception
{
	public ShepherdException(string message) : base(message)
	{
	}

	public ShepherdException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
//ncrunch: no coverage end
