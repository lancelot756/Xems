using Microsoft.AspNetCore.Mvc;
using Xems.Api.Models;
using Xems.Application.Services;
using Xems.Domain.Entities;
using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace Xems.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ElevatorsController : ControllerBase
{
	private readonly ElevatorService _elevatorService;

	public ElevatorsController(ElevatorService elevatorService)
	{
		_elevatorService = elevatorService;
	}

	[HttpGet("elevators")]
	public IActionResult GetAllElevators()
	{
		var elevators = _elevatorService.GetAll();

		return Ok(elevators);
	}

	[HttpGet("elevators/{id}")]
	public IActionResult GetElevatorById(string id)
	{
		var elevator = _elevatorService.GetById(id);

		if (elevator is null)
			return NotFound($"Elevator '{id}' was not found.");

		return Ok(elevator);
	}

	[HttpPost("requests")]
	public IActionResult CreateElevatorRequest(ElevatorRequestDto dto)
	{
		var request = new ElevatorRequest(
				new Floor(dto.FromFloor!.Value),
				dto.Direction!.Value);

		var assignedElevator = _elevatorService.RequestElevator(request);

		if (assignedElevator is null)
			return Conflict("No available elevator found.");

		return Ok(assignedElevator); 
	}

	[Authorize(Roles = "Admin,Operator")]
	[HttpPost("elevators/{id}/maintenance")]
	public IActionResult SetMaintenanceMode(string id, MaintenanceRequestDto dto)
	{
		var elevatorWasFound = _elevatorService.SetMaintenanceMode(id, dto.Enable);

		if (!elevatorWasFound)
			return NotFound($"Elevator '{id}' was not found.");

		return NoContent();
	}

	
}