using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository.Models;
using Repository.Repo;
using Services.Configuration;
using Services.Interface;
using Services.Request;
using Services.Response;
using Stripe;
using Stripe.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SiteDonationSerivces : ISiteDonationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentServices _paymentService;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<SiteDonationSerivces> _logger;

        public SiteDonationSerivces(
            IUnitOfWork unitOfWork,
            IPaymentServices paymentService,
            IOptions<StripeSettings> stripeSettings,
            ILogger<SiteDonationSerivces> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;
        }

        public async Task<CreateDonationResponseDto> PrepareDonationAsync(CreateDonationRequestDto request, Guid? userId, CancellationToken cancellationToken)
        {
            var metadata = new Dictionary<string, string>
        {
            { "paymentType", "SiteDonation" }, // <-- Chìa khóa để webhook phân loại
            { "userId", userId?.ToString() ?? string.Empty },
            { "isAnonymous", request.IsAnonymous.ToString() },
            { "message", request.Message ?? string.Empty }
        };

            var clientSecret = await _paymentService.CreatePaymentIntentAsync(request.Amount, "vnd", metadata, cancellationToken);

            return new CreateDonationResponseDto
            {
                ClientSecret = clientSecret,
                PublishableKey = _stripeSettings.PublishableKey
            };
        }

        public async Task FulfillDonationAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fulfilling site donation for PaymentIntent: {Id}", paymentIntent.Id);

            var existingPayment = await _unitOfWork.PaymentRepository.FindByExternalIdAsync(paymentIntent.Id);
            if (existingPayment != null)
            {
                _logger.LogWarning("Donation PaymentIntent {Id} has already been processed.", paymentIntent.Id);
                return;
            }

            var metadata = paymentIntent.Metadata;
            Guid.TryParse(metadata["userId"], out var userIdFromMetadata);
            bool.TryParse(metadata["isAnonymous"], out var isAnonymous);
            var message = metadata["message"];

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

            var newDonation = new SiteDonation
            {
                Id = Guid.NewGuid(),
                DonorId = (isAnonymous || userIdFromMetadata == Guid.Empty) ? null : userIdFromMetadata,
                Amount = newPayment.Amount,
                PaymentId = newPayment.Id,
                Status = "Paid", 
                Message = message,
                CreatedAt = DateTime.UtcNow,
            };
            _unitOfWork.SiteDonationRepository.Create(newDonation);

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Successfully created SiteDonation {Id}", newDonation.Id);
        }
    }
}
