using Xems.Domain.Entities;

namespace Xems.Application.Services
{
	public interface IElevatorDispatcher
	{
		Elevator? SelectElevator(List<Elevator> elevators, ElevatorRequest request);
		void ApplyLobbyPreference(List<Elevator> elevators);
	}
}
