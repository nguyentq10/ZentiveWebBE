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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);
            _logger.LogInformation("--> Stripe Webhook Event Received: {EventType}", stripeEvent.Type);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) { return Ok(); }

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    // === LOGIC ĐỊNH TUYẾN ===
                    if (paymentIntent.Metadata.TryGetValue("paymentType", out var type) && type == "SiteDonation")
                    {
                        await _donationService.FulfillDonationAsync(paymentIntent, cancellationToken);
                    }
                    else // Mặc định là Pledge
                    {
                        await _pledgeService.FulfillPledgeAsync(paymentIntent, cancellationToken);
                    }
                    break;

                case "payment_intent.payment_failed":
                    await _pledgeService.HandleFailedPledgeAsync(paymentIntent, cancellationToken);
                    break;

                default:
                    _logger.LogWarning("--> Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe Webhook Error: Invalid signature.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe Webhook Error: Processing failed.");
            return StatusCode(500, "Internal server error");
        }
    }
}