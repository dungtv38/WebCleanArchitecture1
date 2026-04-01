using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RoomTypeController : ControllerBase
{
    private readonly IRoomTypeService _service;

    public RoomTypeController(IRoomTypeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomTypeRequest request)
    {
        await _service.CreateAsync(request);
        return Ok();
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetByHotel(Guid hotelId)
    {
        return Ok(await _service.GetByHotelIdAsync(hotelId));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoomTypeRequest request)
    {
        await _service.UpdateAsync(id, request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}