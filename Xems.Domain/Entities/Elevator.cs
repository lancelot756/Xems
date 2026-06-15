using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Domain.Entities
{
	public class Elevator
	{
		public string Id { get; private set; }

		public ElevatorGroup Group { get; private set; }
		public ElevatorState State { get; private set; }
		public ElevatorDirection? Direction { get; private set; }
		public Floor? TargetFloor { get; private set; }
		public Floor CurrentFloor { get; private set; }

		public bool IsAvailable => State is not ElevatorState.Maintenance and not ElevatorState.OutOfService;

		public Elevator(string id, ElevatorGroup group, Floor currentFloor)
		{
			Id = id;
			Group = group;
			State = ElevatorState.Idle;
			Direction = null;
			TargetFloor = null;
			CurrentFloor = currentFloor;
		}

		public void SendToFloor(Floor floor)
		{
			if (!IsAvailable)
				throw new InvalidOperationException("Elevator is not available.");

			TargetFloor = floor;

			if (floor.Value == CurrentFloor.Value)
			{
				State = ElevatorState.Idle;
				Direction = null;
				return;
			}

			Direction = floor.Value > CurrentFloor.Value
					? Enums.ElevatorDirection.Up
					: Enums.ElevatorDirection.Down;

			State = Direction == Enums.ElevatorDirection.Up
					? ElevatorState.MovingUp
					: ElevatorState.MovingDown;
		}

		public void EnterMaintenanceMode()
		{
			State = ElevatorState.Maintenance;
			Direction = null;
			TargetFloor = null;
		}

		public void ExitMaintenanceMode()
		{
			Reactivate();
		}

		public void TakeOutOfService()
		{
			State = ElevatorState.OutOfService;
			Direction = null;
			TargetFloor = null;
		}

		public void Reactivate()
		{
			State = ElevatorState.Idle;
		}
	}
}
