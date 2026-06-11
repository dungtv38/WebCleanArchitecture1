using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc phải truyền JWT Token trong Header mới gọi được
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                var payment = await _paymentService.ProcessPaymentAsync(request);

                // Trả về kết quả thành công cho Frontend nhận diện
                return Ok(new
                {
                    message = request.PaymentMethod.ToUpper() == "COUNTER"
                        ? "Đăng ký đặt giữ phòng thành công! Vui lòng thanh toán tại quầy khi check-in."
                        : "Thanh toán qua cổng trực tuyến thành công!",
                    paymentId = payment.Id,
                    status = payment.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 kèm câu báo lỗi nghiệp vụ cụ thể (Ví dụ: Đơn phòng đã hủy,...)
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}