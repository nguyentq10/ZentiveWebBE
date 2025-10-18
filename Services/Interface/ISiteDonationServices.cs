using Services.Request;
using Services.Response;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISiteDonationServices
    {
       
        Task<CreateDonationResponseDto> PrepareDonationAsync(CreateDonationRequestDto request, Guid? userId, CancellationToken cancellationToken);
        Task FulfillDonationAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken);
    }
}
