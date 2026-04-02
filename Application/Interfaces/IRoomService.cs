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
        Task<Room> CreateAsync(CreateRoomRequest request, Guid currentUserId);
        Task<List<Room>> GetByRoomTypeIdAsync(Guid roomTypeId);
        Task<bool> UpdateStatusAsync(Guid roomId, RoomStatus status, Guid currentUserId);
        Task<bool> DeleteAsync(Guid roomId, Guid currentUserId);
    }
}
