using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;
using Xems.Domain.Exceptions;

namespace Xems.Domain.Entities
{
	public class Elevator
	{
		public string Id { get; private set; }

		public ElevatorGroup Group { get; private set; }
		public ElevatorState State { get; set; }
		public ElevatorDirection? Direction { get; private set; }
		public Floor? TargetFloor { get; private set; }
		public Floor CurrentFloor { get; private set; }

		public bool IsAvailable => State is not ElevatorState.Maintenance and not ElevatorState.OutOfService;

		public Elevator(string id, ElevatorGroup group, Floor currentFloor)
		{
			Id = id;
			Group = group;
			CurrentFloor = currentFloor;
			State = ElevatorState.Idle;
			Direction = null;
			TargetFloor = null;
		}

		public void SendToFloor(Floor floor)
		{
			if (!IsAvailable)
				throw new InvalidElevatorStateTransitionException(
					$"Elevator {Id} cannot be sent to a floor while state is {State}.");

			TargetFloor = floor;

			if (floor.Value == CurrentFloor.Value)
			{
				State = ElevatorState.DoorsOpen;
				Direction = null;
				TargetFloor = null;
				return;
			}

			Direction = floor.Value > CurrentFloor.Value
					? Enums.ElevatorDirection.Up
					: Enums.ElevatorDirection.Down;

			State = Direction == Enums.ElevatorDirection.Up
					? ElevatorState.MovingUp
					: ElevatorState.MovingDown;
		}

		public void OpenDoors()
		{
			if (State is ElevatorState.Maintenance or ElevatorState.OutOfService)
				throw new InvalidElevatorStateTransitionException(
						$"Elevator {Id} cannot open doors while state is {State}.");

			State = ElevatorState.DoorsOpen;
			Direction = null;
			TargetFloor = null;
		}

		public void CloseDoors()
		{
			if (State != ElevatorState.DoorsOpen)
				throw new InvalidElevatorStateTransitionException(
						$"Elevator {Id} cannot close doors while state is {State}.");

			State = ElevatorState.Idle;
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
