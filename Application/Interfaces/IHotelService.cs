using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IHotelService
    {
        Task CreateAsync(CreateHotelRequest request);
        Task<List<Hotel>> GetAllAsync();
        Task<bool> UpdateAsync(Guid id, UpdateHotelRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
