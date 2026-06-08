using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class HotelImageDTO
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
    }

    // DTO cho phòng cụ thể (Room)
    public class RoomDTO
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; }
        public string Status { get; set; } // Chuyển Enum thành String cho FE dễ đọc
        public string? Note { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }

    // DTO cho Loại phòng (RoomType) - Chứa danh sách RoomDTO
    public class RoomTypeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
      
        public List<RoomDTO> Rooms { get; set; } = new List<RoomDTO>();
    }

    // DTO Tổng cho trang Chi tiết Khách sạn
    public class HotelDetailDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public List<HotelImageDTO> Images { get; set; } = new List<HotelImageDTO>();
        public List<RoomTypeDTO> RoomTypes { get; set; } = new List<RoomTypeDTO>();
    }
}

