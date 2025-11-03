using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Repository.Models;
using Repository.Repo;
using Services.Configuration;
using Services.Core;
using Services.Interface;
using Services.Request;
using Services.Response;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PledgeService : IPledgeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOsService _payOsService; // <-- THAY ĐỔI Ở ĐÂY
        private readonly ILogger<PledgeService> _logger;
        private readonly IConfiguration _config;

        public PledgeService(
            IUnitOfWork unitOfWork,
            IPayOsService payOsService, // <-- THAY ĐỔI Ở ĐÂY
            ILogger<PledgeService> logger,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _payOsService = payOsService;
            _logger = logger;
            _config = config;
        }

        // Cập nhật: Không cần HttpContext
        public async Task<PreparePledgeResponseDto> PreparePledgeAsync(Guid projectId, PreparePledgeRequest request, Guid backerId, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null || project.Status != StatusConstants.ProjectPublished || DateTime.UtcNow > project.EndAt)
            {
                throw new InvalidOperationException("Project is not available for pledging.");
            }

            decimal pledgeAmount;
            if (request.RewardTierId.HasValue)
            {
                var tier = await _unitOfWork.RewardTierRepository.GetByIdAsync(request.RewardTierId.Value);
                if (tier == null || tier.ProjectId != projectId) { throw new ArgumentException("Invalid reward tier."); }
                pledgeAmount = tier.Amount;
            }
            else
            {
                if (!request.Amount.HasValue) { throw new ArgumentException("Amount is required."); }
                pledgeAmount = request.Amount.Value;
            }

            // 1. TẠO MÃ GIAO DỊCH (PayOS dùng long, Ticks quá dài)
            var txnRef = DateTime.Now.Ticks.ToString();
            // Lấy 10 số cuối làm orderCode (kiểu long)
            long orderCodeLong = DateTimeOffset.Now.ToUnixTimeSeconds();
            string orderCodeString = orderCodeLong.ToString();

            // 2. TẠO CÁC BẢN GHI "PENDING"
            var newPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Provider = "PayOS",
                Amount = pledgeAmount,
                ExternalId = orderCodeString, // Lưu orderCode
                Status = StatusConstants.PaymentPending,
                CreatedAt = DateTime.UtcNow,
                Currency = "VND"
            };
            _unitOfWork.PaymentRepository.Create(newPayment);

            var newPledge = new Pledge
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                BackerId = backerId,
                RewardTierId = request.RewardTierId,
                Amount = pledgeAmount,
                PaymentId = newPayment.Id,
                Status = StatusConstants.PaymentPending,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.PledgeRepository.Create(newPledge);

            await _unitOfWork.SaveChangesAsync();

            // 3. TẠO URL THANH TOÁN PAYOS
            var returnUrl = _config["PayOsSettings:ReturnUrl"];
            var cancelUrl = _config["PayOsSettings:CancelUrl"];

            // === SỬA LỖI Ở ĐÂY ===
            // Dùng chính orderCodeString (10 ký tự) làm mô tả.
            // Đây là cách an toàn nhất để đảm bảo mô tả < 25 ký tự.
            var orderInfo = orderCodeString;

            // Gọi service với các kiểu dữ liệu đúng (cho payOS.Net5)
            CreatePaymentResult paymentResult = await _payOsService.CreatePaymentUrlAsync(
                orderCode: orderCodeLong, // `IPayOsService` nhận string
                description: orderInfo,       // <-- Mô tả đã rút ngắn
                amount: (int)pledgeAmount,  // PayOS yêu cầu int
                returnUrl: returnUrl,
                cancelUrl: cancelUrl
            );

            newPayment.RawJson = paymentResult.checkoutUrl;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created PayOS URL for OrderCode {OrderCode}", orderCodeString);

            return new PreparePledgeResponseDto
            {
                PaymentUrl = paymentResult.checkoutUrl
            };
        }
        // Hàm này vẫn giữ lại cho API GET
        public async Task<PaginatedListDto<PledgeDetailsDto>> GetPledgesForProjectAsync(Guid projectId, PledgeQueryParameters queryParams, Guid currentUserId, string currentUserRole)
        {
            // ... (Code map thủ công dùng LINQ .Select() đã viết trước đó) ...
            var (pledgesFromDb, totalCount) = await _unitOfWork.PledgeRepository.GetPledgesForProjectAsync(projectId, queryParams);

            var pledgeDtos = pledgesFromDb.Select(pledge => new PledgeDetailsDto
            {
                Id = pledge.Id,
                Amount = pledge.Amount,
                Status = pledge.Status,
                CreatedAt = pledge.CreatedAt,
                Backer = pledge.Backer != null ? new BackerDto
                {
                    Id = pledge.Backer.Id,
                    FullName = pledge.Backer.FullName,
                    Email = pledge.Backer.Email
                } : null
            }).ToList();

            return new PaginatedListDto<PledgeDetailsDto>(pledgeDtos, queryParams.Page, queryParams.PageSize, totalCount);
        }
    }

}
