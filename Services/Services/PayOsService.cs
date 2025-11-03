using Net.payOS;
using Net.payOS.Types;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayOSSDK = Net.payOS.PayOS; // <-- THÊM DÒNG BÍ DANH
namespace Services.Services
{
    public class PayOsService : IPayOsService
    {
        private readonly PayOSSDK _payOS;

        public PayOsService(PayOSSDK payOS)
        {
            _payOS = payOS;
        }

        public async Task<CreatePaymentResult> CreatePaymentUrlAsync(long orderCode, string description, int amount, string returnUrl, string cancelUrl)
        {
            ItemData item = new ItemData(description, 1, amount);
            List<ItemData> items = new List<ItemData> { item };

            PaymentData paymentData = new PaymentData(
                orderCode: orderCode, // <-- Dùng thẳng kiểu long
                amount: amount,
                description: description,
                items: items,
                cancelUrl: cancelUrl,
                returnUrl: returnUrl
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);
            return result;
        }
    }
}
