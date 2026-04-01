using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task CreateAsync(CreateRoomTypeRequest request);
        Task<List<RoomType>> GetByHotelIdAsync(Guid hotelId);
        Task UpdateAsync(Guid id, UpdateRoomTypeRequest request);
        Task DeleteAsync(Guid id);
    }
}
