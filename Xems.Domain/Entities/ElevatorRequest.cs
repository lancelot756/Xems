using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Domain.Entities
{
	public class ElevatorRequest
	{
		public Floor FromFloor { get; }

		public Direction Direction { get; }

		public ElevatorRequest(Floor fromFloor, Direction direction)
		{
			FromFloor = fromFloor;
			Direction = direction;
		}
	}
}
