using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.payOS.Types;

namespace Services.Interface
{
    public interface IPayOsService
    {
        Task<CreatePaymentResult> CreatePaymentUrlAsync(long orderCode, string description, int amount, string returnUrl, string cancelUrl);
    }
}
