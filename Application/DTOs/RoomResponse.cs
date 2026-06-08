namespace Application.DTOs
{
    public class RoomResponse
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public string RoomNumber { get; set; } = default!;
        public string? Note { get; set; }
        public string Status { get; set; } = default!;

        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }

        public List<string> Images { get; set; } = new();
    }
}