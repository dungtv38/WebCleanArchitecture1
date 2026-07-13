using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessOfflinePayment(CreateOfflinePaymentDto dto);
        Task<string> CreateOnlinePaymentUrl(Guid bookingId, string returnUrl);

        // Xử lý khi cổng thanh toán trả kết quả về
        Task<bool> HandleIpnCallback(string transactionCode, bool isSuccess);

    }
    
}
