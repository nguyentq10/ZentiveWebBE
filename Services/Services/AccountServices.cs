using Azure.Core;
using BCrypt.Net;
using Repository.Models;
using Repository.Repo;
using Services.Interface;
using Services.Request;
using Services.Response;
using System;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        // Chỉ giữ lại constructor này để đảm bảo sử dụng Dependency Injection
        public AccountServices(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            if (await _unitOfWork.AccountRepository.ExistsByEmailAsync(request.Email))
                throw new Exception("Email already registered.");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = request.Address,
                School = request.School,
                Role = "Creator", // Nên dùng một lớp static để định nghĩa Role
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                AvatarUrl = null
            };

            // Bước 1: Chuẩn bị tạo mới Account.
            // Hàm Create này (trong Repository đã sửa) chỉ đưa entity vào DbContext, chưa lưu.
            _unitOfWork.AccountRepository.Create(account);

            // Bước 2: Lưu tất cả các thay đổi vào database trong một transaction.
            await _unitOfWork.SaveChangesAsync();

            return _jwtService.GenerateToken(account);
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Hàm LoginAsync của bạn đã đúng logic nên không cần sửa.
            var account = await _unitOfWork.AccountRepository.GetByEmailAsync(request.Email);

            if (account == null || !BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(account);

            var userProfile = new UserProfileResponse
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Address = account.Address,
                School = account.School,
                Phone = account.Phone,
                AvatarUrl = account.AvatarUrl,
                Role = account.Role
            };

            return new LoginResponse
            {
                Token = token,
                UserProfile = userProfile
            };
        }
    }
}