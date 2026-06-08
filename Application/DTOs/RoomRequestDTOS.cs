using Domain.Entities;

namespace Application.DTOs
{
    public class CreateRoomRequest
    {
        public Guid RoomTypeId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public string? Note { get; set; }

        // 🌟 THÊM 2 TRƯỜNG NÀY VÀO ĐÂY
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }

        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateRoomRequest
    {
        public string RoomNumber { get; set; } = default!;
        public string? Note { get; set; }

        // 🌟 THÊM 2 TRƯỜNG NÀY VÀO ĐÂY ĐỂ CHO PHÉP SỬA GIÁ PHÒNG
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }

        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateRoomStatusRequest
    {
        public RoomStatus Status { get; set; }
    }
}