using Repository.Repo;
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
    public interface IPledgeServices
    {
        Task<PreparePledgeResponseDto> PreparePledgeAsync(Guid projectId, PreparePledgeRequest request, Guid backerId, CancellationToken cancellationToken);
        Task FulfillPledgeAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken);

        Task HandleFailedPledgeAsync(PaymentIntent paymentIntent, CancellationToken cancellationToken);

        Task<PaginatedListDto<PledgeDetailsDto>> GetPledgesForProjectAsync(Guid projectId, PledgeQueryParameters queryParams, Guid currentUserId, string currentUserRole);

    }
}

