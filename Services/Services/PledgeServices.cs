using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository.Models;
using Repository.Repo;
using Services.Configuration;
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
        private readonly IPaymentServices _paymentService;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PledgeService> _logger;
        public PledgeService(
            IUnitOfWork unitOfWork,
            IPaymentServices paymentService,
            IOptions<StripeSettings> stripeSettings,
            ILogger<PledgeService> logger
           )
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
           
        }
        public async Task<PreparePledgeResponseDto> PreparePledgeAsync(Guid projectId, PreparePledgeRequest request, Guid backerId, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null || project.Status != "Published" || DateTime.UtcNow > project.EndAt)
            {
                throw new InvalidOperationException("Project is not available for pledging.");
            }

            // THAY ĐỔI LỚN NHẤT TẠI ĐÂY
            decimal pledgeAmount; // Biến để lưu số tiền cuối cùng

            if (request.RewardTierId.HasValue)
            {
                // Trường hợp 1: Người dùng chọn một gói phần thưởng
                var tier = await _unitOfWork.RewardTierRepository.GetByIdAsync(request.RewardTierId.Value);
                if (tier == null || tier.ProjectId != projectId)
                {
                    throw new ArgumentException("Invalid reward tier for this project.");
                }
                // Tự động gán số tiền của gói, bỏ qua Amount từ request
                pledgeAmount = tier.Amount;
            }
            else
            {
                // Trường hợp 2: Người dùng ủng hộ tự do (không chọn gói)
                // Lúc này, Amount trong request là bắt buộc.
                if (!request.Amount.HasValue)
                {
                    throw new ArgumentException("Amount is required when no reward tier is selected.");
                }
                pledgeAmount = request.Amount.Value;
            }

            var metadata = new Dictionary<string, string>
        {
            { "projectId", projectId.ToString() },
            { "backerId", backerId.ToString() },
            { "rewardTierId", request.RewardTierId?.ToString() ?? string.Empty }
        };

            // Dùng `pledgeAmount` đã được xác định để tạo thanh toán
            var clientSecret = await _paymentService.CreatePaymentIntentAsync(pledgeAmount, "vnd", metadata, cancellationToken);

            return new PreparePledgeResponseDto
            {
                ClientSecret = clientSecret,
                PublishableKey = _stripeSettings.PublishableKey
            };
        }


        public async Task FulfillPledgeAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fulfilling pledge for PaymentIntent: {PaymentIntentId}", paymentIntent.Id);

            var existingPayment = await _unitOfWork.PaymentRepository.FindByExternalIdAsync(paymentIntent.Id);
            if (existingPayment != null)
            {
                _logger.LogWarning("PaymentIntent {PaymentIntentId} has already been processed.", paymentIntent.Id);
                return;
            }

            var metadata = paymentIntent.Metadata;
            Guid.TryParse(metadata["projectId"], out var projectId);
            Guid.TryParse(metadata["backerId"], out var backerId);
            Guid.TryParse(metadata["rewardTierId"], out var rewardTierId);

            if (projectId == Guid.Empty || backerId == Guid.Empty)
            {
                _logger.LogError("PaymentIntent {PaymentIntentId} metadata is missing required information.", paymentIntent.Id);
                throw new InvalidOperationException("PaymentIntent metadata is missing.");
            }

            var newPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Provider = "Stripe",
                Currency = paymentIntent.Currency.ToUpper(),
                Amount = paymentIntent.Amount / (paymentIntent.Currency.ToLower() == "vnd" ? 1 : 100),
                ExternalId = paymentIntent.Id,
                Status = "Paid",
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                RawJson = paymentIntent.ToJson()
            };
            _unitOfWork.PaymentRepository.Create(newPayment);

            var newPledge = new Pledge
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                BackerId = backerId,
                RewardTierId = rewardTierId == Guid.Empty ? null : rewardTierId,
                Amount = newPayment.Amount,
                PaymentId = newPayment.Id,
                Status = "Paid",
                CreatedAt = DateTime.UtcNow,
            };
            _unitOfWork.PledgeRepository.Create(newPledge);

            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project != null)
            {
                project.CurrentAmount += newPledge.Amount;
                _unitOfWork.ProjectRepository.Update(project);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully created Pledge {PledgeId} for Project {ProjectId}", newPledge.Id, projectId);
        }

        public async Task HandleFailedPledgeAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Handling failed pledge for PaymentIntent: {PaymentIntentId}", paymentIntent.Id);

            // 1. KIỂM TRA XEM ĐÃ GHI LẠI LỖI NÀY CHƯA (IDEMPOTENCY)
            var existingPayment = await _unitOfWork.PaymentRepository.FindByExternalIdAsync(paymentIntent.Id);
            if (existingPayment != null)
            {
                _logger.LogWarning("Failed PaymentIntent {PaymentIntentId} has already been recorded.", paymentIntent.Id);
                return; // Đã ghi lại rồi, không làm gì nữa.
            }

            // 2. ĐỌC METADATA ĐỂ BIẾT CONTEXT
            var metadata = paymentIntent.Metadata;
            Guid.TryParse(metadata["backerId"], out var backerId); // Có thể cần backerId để biết ai đã thất bại

            // 3. CHỈ TẠO BẢN GHI PAYMENT, KHÔNG TẠO PLEDGE
            // Một thanh toán thất bại không phải là một lượt ủng hộ (pledge).
            var failedPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Provider = "Stripe",
                Currency = paymentIntent.Currency.ToUpper(),
                Amount = paymentIntent.Amount / (paymentIntent.Currency.ToLower() == "vnd" ? 1 : 100),
                ExternalId = paymentIntent.Id,
                Status = "Failed", // <-- SỬ DỤNG TRẠNG THÁI "Failed" (hoặc trạng thái tương ứng trong DB của bạn)
                PaidAt = null, // Không có ngày thanh toán thành công
                CreatedAt = DateTime.UtcNow,
                // Lưu lại thông tin lỗi từ Stripe
                RawJson = paymentIntent.LastPaymentError?.Message ?? paymentIntent.ToJson()
            };

            _unitOfWork.PaymentRepository.Create(failedPayment);

            // 4. LƯU THAY ĐỔI VÀO DATABASE
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully recorded failed payment {PaymentId}", failedPayment.Id);
        }
        public async Task<PaginatedListDto<PledgeDetailsDto>> GetPledgesForProjectAsync(Guid projectId, PledgeQueryParameters queryParams, Guid currentUserId, string currentUserRole)
        {
            // === BƯỚC 1: KIỂM TRA PHÂN QUYỀN (giữ nguyên) ===
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            if (currentUserRole != "Admin" && project.CreatorId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view pledges for this project.");
            }

            // === BƯỚC 2: GỌI REPOSITORY ĐỂ LẤY DỮ LIỆU (giữ nguyên) ===
            var (pledgesFromDb, totalCount) = await _unitOfWork.PledgeRepository.GetPledgesForProjectAsync(projectId, queryParams);

            // === BƯỚC 3: MAP THỦ CÔNG TỪ ENTITY SANG DTO ===
            var pledgeDtos = new List<PledgeDetailsDto>();
            foreach (var pledge in pledgesFromDb)
            {
                pledgeDtos.Add(new PledgeDetailsDto
                {
                    Id = pledge.Id,
                    Amount = pledge.Amount,
                    Status = pledge.Status,
                    CreatedAt = pledge.CreatedAt,
                    Backer = pledge.Backer != null ? new BackerDto // Kiểm tra backer null cho an toàn
                    {
                        Id = pledge.Backer.Id,
                        FullName = pledge.Backer.FullName,
                        Email = pledge.Backer.Email
                    } : null
                });
            }

            // === BƯỚC 4: TẠO VÀ TRẢ VỀ KẾT QUẢ PHÂN TRANG (giữ nguyên) ===
            return new PaginatedListDto<PledgeDetailsDto>(pledgeDtos, queryParams.Page, queryParams.PageSize, totalCount);
        }

    }

}
