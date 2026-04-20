using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Mở comment này nếu bạn muốn bảo mật API bằng JWT
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // 1. Lấy danh sách phòng theo loại phòng
        [HttpGet("type/{roomTypeId}")]
        public async Task<IActionResult> GetByRoomType(Guid roomTypeId)
        {
            var rooms = await _roomService.GetByRoomTypeIdAsync(roomTypeId);
            return Ok(rooms);
        }

        // 2. Tạo phòng mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
        {
            try
            {
                var room = await _roomService.CreateAsync(request);
                // Trả về 201 Created kèm thông tin phòng vừa tạo
                return CreatedAtAction(nameof(GetByRoomType), new { roomTypeId = room.RoomTypeId }, room);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. Cập nhật thông tin phòng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request)
        {
            try
            {
                var result = await _roomService.UpdateAsync(id, request);
                if (result) return Ok(new { message = "Cập nhật phòng thành công" });
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. Cập nhật trạng thái phòng (ví dụ: Chuyển sang "Đang dọn dẹp" hoặc "Đang bảo trì")
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] RoomStatus status)
        {
            try
            {
                var result = await _roomService.UpdateStatusAsync(id, status);
                if (result) return Ok(new { message = "Cập nhật trạng thái thành công" });
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. Xóa phòng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _roomService.DeleteAsync(id);
                if (result) return Ok(new { message = "Xóa phòng thành công" });
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}