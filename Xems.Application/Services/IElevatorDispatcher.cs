using Xems.Domain.Entities;

namespace Xems.Application.Services
{
	public interface IElevatorDispatcher
	{
		Elevator? SelectElevator(IEnumerable<Elevator> elevators, ElevatorRequest request);
	}
}
