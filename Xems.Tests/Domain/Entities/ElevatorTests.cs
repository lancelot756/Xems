using Xems.Domain.Entities;
using Xems.Domain.Enums;
using Xems.Domain.Exceptions;
using Xems.Domain.ValueObjects;

namespace Xems.Tests.Domain.Entities;

public class ElevatorTests
{
	[Fact]
	public void NewElevator_ShouldStartAsIdle()
	{
		var elevator = new Elevator("A1", ElevatorGroup.A, new Floor(0));

		Assert.Equal(ElevatorState.Idle, elevator.State);
		Assert.Null(elevator.Direction);
		Assert.Null(elevator.TargetFloor);
		Assert.True(elevator.IsAvailable);
	}

	[Fact]
	public void SendToFloor_WhenTargetIsAboveCurrentFloor_ShouldMoveUp()
	{
		var elevator = new Elevator("A1", ElevatorGroup.A, new Floor(0));

		elevator.SendToFloor(new Floor(5));

		Assert.Equal(ElevatorState.MovingUp, elevator.State);
		Assert.Equal(ElevatorDirection.Up, elevator.Direction);
		Assert.Equal(5, elevator.TargetFloor!.Value);
	}

	[Fact]
	public void SendToFloor_WhenElevatorIsInMaintenance_ShouldThrowException()
	{
		var elevator = new Elevator("A1", ElevatorGroup.A, new Floor(0));
		elevator.EnterMaintenanceMode();

		Assert.Throws<InvalidElevatorStateTransitionException>(() =>
				elevator.SendToFloor(new Floor(5)));
	}

	[Fact]
	public void EnterMaintenanceMode_ShouldMakeElevatorUnavailable()
	{
		var elevator = new Elevator("A1", ElevatorGroup.A, new Floor(0));

		elevator.EnterMaintenanceMode();

		Assert.Equal(ElevatorState.Maintenance, elevator.State);
		Assert.Null(elevator.Direction);
		Assert.Null(elevator.TargetFloor);
		Assert.False(elevator.IsAvailable);
	}

	[Fact]
	public void Floor_WhenValueIsOutsideAllowedRange_ShouldThrowException()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => new Floor(13));
		Assert.Throws<ArgumentOutOfRangeException>(() => new Floor(-3));
	}
}