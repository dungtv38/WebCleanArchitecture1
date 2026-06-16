using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        // Thay đổi kiểu trả về từ Task<Payment> thành Task<string>
        Task<string> CreatePaymentUrlAsync(Guid bookingId, string paymentMethod, string ipAddress);
    }
}
