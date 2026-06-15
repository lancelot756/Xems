using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Domain.Entities
{
	public class Elevator
	{
		public string Id { get; private set; }

		public ElevatorGroup Group { get; private set; }

		public Floor CurrentFloor { get; private set; }

		public ElevatorState State { get; private set; }

		public Direction? Direction { get; private set; }

		public Floor? TargetFloor { get; private set; }

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
	}
}
