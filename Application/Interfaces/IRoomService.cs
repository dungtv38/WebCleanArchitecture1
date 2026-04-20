using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoomService
    {
        Task<RoomResponse> CreateAsync(CreateRoomRequest request);
        Task<List<Room>> GetByRoomTypeIdAsync(Guid roomTypeId);
        Task<bool> UpdateStatusAsync(Guid roomId, RoomStatus status);
        Task<bool> DeleteAsync(Guid roomId);
        Task<bool> UpdateAsync(Guid roomId, UpdateRoomRequest request);
    }
}
