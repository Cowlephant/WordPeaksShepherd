namespace WordlePeaksShepherd.Services.Interfaces
{
	public class Word : ValueObject
	{
		public string Value { get; private set; }
		public int Score { get; private set; }

		public Word(string value, int score)
		{
			Value = value;
			Score = score;
		}
	}
}
