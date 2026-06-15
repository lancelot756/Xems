using Xems.Domain.Enums;
using Xems.Domain.ValueObjects;

namespace Xems.Domain.Entities
{
	public class ElevatorRequest
	{
		public Floor FromFloor { get; }

		public ElevatorDirection Direction { get; }

		public ElevatorRequest(Floor fromFloor, ElevatorDirection direction)
		{
			FromFloor = fromFloor;
			Direction = direction;
		}
	}
}
