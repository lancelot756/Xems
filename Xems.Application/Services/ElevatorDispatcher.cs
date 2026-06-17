using Xems.Domain.Entities;
using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Application.Services
{
	public class ElevatorDispatcher : IElevatorDispatcher
	{
		public Elevator? SelectElevator(List<Elevator> elevators, ElevatorRequest request)
		{
			return elevators
					.Where(elevator => elevator.IsAvailable) // fjerner heiser som er i Maintenance eller OutOfService
					.OrderBy(elevator => CalculateCost(elevator, request))
					.FirstOrDefault();
		}

		#region Beregn kostnad
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
			if (elevator.State == ElevatorState.DoorsOpen)
				return 1;

			if (elevator.Direction == request.Direction && IsMovingTowardsRequestFloor(elevator, request))
				return -2;

			return 10;
		}

		private static bool IsMovingTowardsRequestFloor(Elevator elevator, ElevatorRequest request)
		{
			if (elevator.Direction == ElevatorDirection.Up)
				return elevator.CurrentFloor.Value <= request.FromFloor.Value;

			if (elevator.Direction == ElevatorDirection.Down)
				return elevator.CurrentFloor.Value >= request.FromFloor.Value;

			return false;
		}
		#endregion

		#region Lobby preference

		private const int LobbyFloor = 0;
		private const int RequiredLobbyElevators = 4;
		private bool CanReturnToLobby(Elevator elevator)
		{
			return elevator.State == ElevatorState.Idle && elevator.CurrentFloor.Value != LobbyFloor;
		}

		public void ApplyLobbyPreference(List<Elevator> elevators)
		{
			// Tell hvor mange heiser som allerede er i lobbyen
			var lobbyCount = 0;
			foreach (var elevator in elevators)
			{
				var isInLobby = elevator.CurrentFloor.Value == LobbyFloor;
				var isIdle = elevator.State == ElevatorState.Idle;
				if (isInLobby && isIdle)
					lobbyCount++;
			}

			// Hvis 4 heiser allerede er i lobbyen, gjør ingenting
			if (lobbyCount >= RequiredLobbyElevators)
				return;

			// Ta de nærmeste, maks 4
			var candidates = elevators
					.Where(CanReturnToLobby)
					.OrderBy(e => Math.Abs(e.CurrentFloor.Value - LobbyFloor))
					.Take(RequiredLobbyElevators - lobbyCount);

			foreach (var elevator in candidates)
				elevator.SendToFloor(new Floor(LobbyFloor));
		}

		#endregion

	}
}