using FluentAssertions;
using Xems.Application.Services;
using Xems.Domain.Entities;
using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Tests.Application.Services;

public class ElevatorDispatcherTests
{
	private readonly ElevatorDispatcher _dispatcher = new();

	[Fact]
	public void SelectElevator_WhenAllElevatorsAreIdle_ShouldSelectNearestElevator()
	{
		var request = new ElevatorRequest(new Floor(6), ElevatorDirection.Up);

		var elevators = new List<Elevator>
				{
						new("A1", ElevatorGroup.A, new Floor(0)),
						new("A2", ElevatorGroup.A, new Floor(4)),
						new("A3", ElevatorGroup.A, new Floor(10))
				};

		var selectedElevator = _dispatcher.SelectElevator(elevators, request);

		selectedElevator.Should().NotBeNull();
		selectedElevator!.Id.Should().Be("A2");
	}

	[Fact]
	public void SelectElevator_WhenElevatorIsMovingTowardsRequest_ShouldPreferThatElevator()
	{
		var request = new ElevatorRequest(new Floor(6), ElevatorDirection.Up);

		var movingElevator = new Elevator("A1", ElevatorGroup.A, new Floor(4));
		movingElevator.SendToFloor(new Floor(10));

		var idleElevator = new Elevator("A2", ElevatorGroup.A, new Floor(9));

		var elevators = new List<Elevator>
				{
						movingElevator,
						idleElevator
				};

		var selectedElevator = _dispatcher.SelectElevator(elevators, request);

		selectedElevator.Should().Be(movingElevator);
	}

	[Fact]
	public void SelectElevator_WhenElevatorIsInMaintenance_ShouldNotSelectIt()
	{
		var request = new ElevatorRequest(new Floor(5), ElevatorDirection.Up);

		var maintenanceElevator = new Elevator("A1", ElevatorGroup.A, new Floor(5));
		maintenanceElevator.EnterMaintenanceMode();

		var availableElevator = new Elevator("A2", ElevatorGroup.A, new Floor(8));

		var elevators = new List<Elevator>
				{
						maintenanceElevator,
						availableElevator
				};

		var selectedElevator = _dispatcher.SelectElevator(elevators, request);

		selectedElevator.Should().Be(availableElevator);
	}

	[Fact]
	public void ApplyLobbyPreference_WhenFewerThanFourElevatorsAreInLobby_ShouldSendNearestIdleElevatorsToLobby()
	{
		var elevators = new List<Elevator>
				{
						new("A1", ElevatorGroup.A, new Floor(0)),
						new("A2", ElevatorGroup.A, new Floor(0)),
						new("A3", ElevatorGroup.A, new Floor(2)),
						new("A4", ElevatorGroup.A, new Floor(8)),
						new("B1", ElevatorGroup.B, new Floor(12))
				};

		_dispatcher.ApplyLobbyPreference(elevators);

		elevators.Single(e => e.Id == "A3").TargetFloor!.Value.Should().Be(0);
		elevators.Single(e => e.Id == "A4").TargetFloor!.Value.Should().Be(0);

		elevators.Single(e => e.Id == "B1").TargetFloor.Should().BeNull();
	}

	[Fact]
	public void ApplyLobbyPreference_WhenFourElevatorsAreAlreadyInLobby_ShouldNotSendMoreElevatorsToLobby()
	{
		var elevators = new List<Elevator>
				{
						new("A1", ElevatorGroup.A, new Floor(0)),
						new("A2", ElevatorGroup.A, new Floor(0)),
						new("A3", ElevatorGroup.A, new Floor(0)),
						new("A4", ElevatorGroup.A, new Floor(0)),
						new("B1", ElevatorGroup.B, new Floor(7))
				};

		_dispatcher.ApplyLobbyPreference(elevators);

		elevators.Single(e => e.Id == "B1").TargetFloor.Should().BeNull();
		elevators.Single(e => e.Id == "B1").State.Should().Be(ElevatorState.Idle);
	}
}