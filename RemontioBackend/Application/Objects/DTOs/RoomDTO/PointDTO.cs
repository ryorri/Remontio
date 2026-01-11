using Application.Interfaces.AdditionalInterfaces;

namespace Application.Objects.DTOs.RoomDTO
{
    public class PointDTO : IPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}
