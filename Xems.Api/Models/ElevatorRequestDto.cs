using System.ComponentModel.DataAnnotations;
using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Api.Models;

public class ElevatorRequestDto
{
	[Required]
	[Range(Floor.MinValue, Floor.MaxValue, ErrorMessage = "Floor must be between -2 and 12.")]
	public int? FromFloor { get; set; }

	[Required(ErrorMessage = "Direction is required.")]
	public ElevatorDirection? Direction { get; set; }
}