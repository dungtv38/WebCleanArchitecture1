using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });

                Guid userId = Guid.Parse(userIdClaim);

                // Chạy logic lưu DB thành công
                var booking = await _bookingService.CreateAsync(userId, request);

                var resultDto = new
                {
                    Id = booking.Id,
                    HotelId = booking.HotelId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status.ToString(),
                    CreatedAt = booking.CreatedAt,
                    // Chỉ lấy các thông tin phòng cần thiết, tuyệt đối không chấm ngược lại .Booking
                    Rooms = booking.BookingDetails.Select(bd => new {
                        RoomId = bd.RoomId,
                        Nights = bd.Nights,
                        PricePerNight = bd.PricePerNight
                    }).ToList()
                };

                // Trả về DTO đã bóc tách sạch sẽ
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }// yêu cầu đăng nhập

    }
}