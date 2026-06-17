using Xems.Domain.Entities;
using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Application.Services;

public class ElevatorService
{
	private readonly ElevatorDispatcher _dispatcher;

	private readonly List<Elevator> _elevators =
	[
			new("A1", ElevatorGroup.A, new Floor(0)),
			new("A2", ElevatorGroup.A, new Floor(0)),
			new("A3", ElevatorGroup.A, new Floor(0)),
			new("A4", ElevatorGroup.A, new Floor(0)),
			new("B1", ElevatorGroup.B, new Floor(0)),
			new("B2", ElevatorGroup.B, new Floor(0)),
			new("B3", ElevatorGroup.B, new Floor(0)),
			new("B4", ElevatorGroup.B, new Floor(0))
	];

	public ElevatorService(ElevatorDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	public IReadOnlyList<Elevator> GetAll()
	{
		return _elevators;
	}

	public Elevator? GetById(string id)
	{
		return _elevators.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
	}

	public Elevator? RequestElevator(ElevatorRequest request)
	{
		var elevator = _dispatcher.SelectElevator(_elevators, request);

		if (elevator is null)
			return null;

		elevator.SendToFloor(request.FromFloor);

		_dispatcher.ApplyLobbyPreference(_elevators);

		return elevator;
	}

	public bool SetMaintenanceMode(string id, bool enable)
	{
		var elevator = GetById(id);

		if (elevator is null)
			return false;

		if (enable)
			elevator.EnterMaintenanceMode();
		else
			elevator.ExitMaintenanceMode();

		_dispatcher.ApplyLobbyPreference(_elevators);

		return true;
	}
}