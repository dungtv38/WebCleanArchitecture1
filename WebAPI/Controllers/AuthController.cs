using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context,IAuthService authService)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }
    
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        return Ok(
            await _authService.RefreshTokenAsync(
                refreshToken
            )
        );
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(
                refreshToken
            );
        }

        Response.Cookies.Delete(
            "refreshToken"
        );

        return Ok(new
        {
            message = "Logged out successfully"
        });
    }
    // ✅ API LẤY THÔNG TIN HỒ SƠ CÁ NHÂN
    [Authorize] // Ép buộc phải có Token JWT hợp lệ gửi kèm trên Header
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            // 1. Tìm UserId nằm ẩn trong các Claim của mã JWT Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin xác thực người dùng." });
            }

            Guid userId = Guid.Parse(userIdClaim);

            // 2. Tìm kiếm thông tin User trong SQL Server thông qua DbContext
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại trên hệ thống." });
            }

            // 3. Map dữ liệu sang DTO sạch sẽ để trả về cho Frontend React
            var response = new UserProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName, // Hoặc trường tên tương ứng trong DB của bạn
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra: " + ex.Message });
        }
    }




    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // localhost
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return Ok(new
        {
            accessToken = result.AccessToken
        });
    }
}