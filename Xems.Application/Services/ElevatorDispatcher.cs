using Xems.Domain.Entities;
using Xems.Domain.Enums;

namespace Xems.Application.Services
{
	public class ElevatorDispatcher : IElevatorDispatcher
	{
		public Elevator? SelectElevator(IEnumerable<Elevator> elevators, ElevatorRequest request)
		{
			return elevators
					.Where(elevator => elevator.IsAvailable) // fjerner heiser som er i Maintenance eller OutOfService
					.OrderBy(elevator => CalculateCost(elevator, request))
					.FirstOrDefault();
		}

		private static int CalculateCost(Elevator elevator, ElevatorRequest request)
		{
			var distanceCost = Math.Abs(elevator.CurrentFloor.Value - request.FromFloor.Value);
			var directionCost = CalculateDirectionCost(elevator, request);
			return distanceCost + directionCost;
		}

		private static int CalculateDirectionCost(Elevator elevator, ElevatorRequest request)
		{
			if (elevator.State == ElevatorState.Idle)
				return 0;

			if (elevator.Direction == request.Direction && IsMovingTowardsRequestFloor(elevator, request))
				return -2;

			return 10;
		}

		private static bool IsMovingTowardsRequestFloor(Elevator elevator, ElevatorRequest request)
		{
			if (elevator.Direction == Direction.Up)
				return elevator.CurrentFloor.Value <= request.FromFloor.Value;

			if (elevator.Direction == Direction.Down)
				return elevator.CurrentFloor.Value >= request.FromFloor.Value;

			return false;
		}
	}
}