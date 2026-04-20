using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        // Tạo đơn đặt phòng mới (Đã sửa kiểu trả về thành Task<Booking>)
        Task<Booking> CreateAsync(Guid userId, CreateBookingRequest request);

        // Lấy chi tiết một đơn hàng
        //Task<Booking?> GetByIdAsync(Guid id);

        //// Lấy lịch sử đặt phòng của một User
        //Task<List<Booking>> GetUserBookingHistoryAsync(Guid userId);

        //// Hủy đơn đặt phòng
        //Task<bool> CancelBookingAsync(Guid bookingId, Guid userId);

        //// Cập nhật trạng thái thanh toán (Dùng cho sau này tích hợp VNPay/Momo)
        //Task<bool> UpdatePaymentStatusAsync(Guid bookingId, BookingStatus status);
    }
}
