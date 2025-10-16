using Microsoft.Extensions.Configuration;
using Repository.Models;
using Repository.Repo;
using Services.Interface;
using Services.Request;
using Services.Response;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PaymentServices : IPaymentServices
    {
        public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency, Dictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            long stripeAmount = (long)amount;
            if (currency.ToLower() != "vnd")
            {
                stripeAmount = (long)(amount * 100);
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = stripeAmount,
                Currency = currency.ToLower(),
                Metadata = metadata, 
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return intent.ClientSecret;
        }
    }
}

