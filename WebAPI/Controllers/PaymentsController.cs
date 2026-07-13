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
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("offline")]
        public async Task<IActionResult> PayOffline([FromBody] CreateOfflinePaymentDto request)
        {
            try
            {
                var success = await _paymentService.ProcessOfflinePayment(request);

                return Ok(new
                {
                    message = "Thanh toán thành công",
                    success
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
    string transactionCode,
    bool success)
        {
            var result = await _paymentService
                .HandleIpnCallback(
                    transactionCode,
                    success
                );


            return Ok(result);
        }
    }
}