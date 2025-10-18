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

        public AccountServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            // Gọi phương thức count từ Repository
            return await _unitOfWork.AccountRepository.CountActiveUsersAsync();
        }
    }
}