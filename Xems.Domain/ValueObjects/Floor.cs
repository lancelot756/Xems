namespace Xems.Domain.ValueObjects
{
	public class Floor
	{
		public const int MinValue = -2;
		public const int MaxValue = 12;

		public int Value { get; }

		public Floor(int value)
		{
			if (value < MinValue || value > MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Floor must be between {MinValue} and {MaxValue}.");
			}

			Value = value;
		}
	}
}
