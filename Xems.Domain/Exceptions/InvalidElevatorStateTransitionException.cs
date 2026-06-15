namespace Xems.Domain.Exceptions;

public class InvalidElevatorStateTransitionException : Exception
{
	public InvalidElevatorStateTransitionException(string message) : base(message)
	{
	}
}