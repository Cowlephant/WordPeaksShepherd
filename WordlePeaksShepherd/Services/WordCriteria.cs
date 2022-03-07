namespace WordlePeaksShepherd.Services;

public sealed class WordCriteria : ValueObject
{
	public List<LetterCriteria> LetterCriteria { get; private set; }

	[IgnoreMember]
	public bool IsSolved => LetterCriteria.All(criteria => criteria.IsCorrect);

	public WordCriteria(IEnumerable<LetterCriteria> letterCriteria)
	{
		var isOutOfRange = letterCriteria.Count() != 5;
		if (isOutOfRange)
		{
			var exceptionMessage = $"Expected 5 letters, received {letterCriteria.Count()}.";
			throw new ArgumentOutOfRangeException(nameof(letterCriteria), exceptionMessage); 
		}

		LetterCriteria = letterCriteria.ToList();
	}
}
