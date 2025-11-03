using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interface;
using Services.Services;
using Stripe; // Vẫn cần using Stripe
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers;

[Route("api/stripe-webhooks")]
[ApiController]
public class StripeWebhooksController : ControllerBase
{
    private readonly IPledgeServices _pledgeService;
    private readonly ILogger<StripeWebhooksController> _logger;
    private readonly ISiteDonationServices _donationService; 
    private readonly string _webhookSecret;

    public StripeWebhooksController(
        IPledgeServices pledgeService,ISiteDonationServices donationService,
        ILogger<StripeWebhooksController> logger,
        IConfiguration config)
    {
        _pledgeService = pledgeService;
        _logger = logger;
        _donationService = donationService;
        _webhookSecret = config["StripeSettings:WebhookSecret"];
    }
}