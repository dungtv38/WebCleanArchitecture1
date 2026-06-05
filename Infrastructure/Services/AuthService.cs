using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials");
            }

            var accessToken = _jwtService.GenerateToken(user);

            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Kiểm tra trùng email
            var userExists = await _context.Users.AnyAsync(x => x.Email == request.Email);
            if (userExists) throw new Exception("Email đã tồn tại");

            // 2. Tạo User mới với Role động từ Request
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Trả về Token (JwtService của bạn sẽ tự động bốc Role này vào Claim)
            return new AuthResponse
            {
                AccessToken = _jwtService.GenerateToken(user)
            };
        }
        public async Task<AuthResponse>
    RefreshTokenAsync(
        string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshToken &&
                    !x.IsRevoked);

            if (token == null)
                throw new Exception(
                    "Invalid refresh token");

            if (token.ExpiresAt <
                DateTime.UtcNow)
                throw new Exception(
                    "Refresh token expired");

            return new AuthResponse
            {
                AccessToken =
                    _jwtService.GenerateToken(
                        token.User
                    )
            };
        }

        public async Task LogoutAsync(
     string refreshToken)
        {
            var token =
                await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshToken);

            if (token != null)
            {
                token.IsRevoked = true;

                await _context.SaveChangesAsync();
            }
        }
    }
}
