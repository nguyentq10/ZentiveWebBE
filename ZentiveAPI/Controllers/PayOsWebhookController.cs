using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Repository.Repo;
using Services.Core;
using Net.payOS.Types;
using PayOSSDK = Net.payOS.PayOS;
using System.IO; // <-- Thêm
using Newtonsoft.Json; // <-- Thêm
namespace ZentiveAPI.Controllers
{
    [ApiController]
    [Route("api/payos-webhook")]
    public class PayOsWebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PayOsWebhookController> _logger;
        private readonly PayOSSDK _payOS;

        public PayOsWebhookController(
            IUnitOfWork unitOfWork,
            ILogger<PayOsWebhookController> logger,
            PayOSSDK payOS)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _payOS = payOS;
        }

        [HttpPost]
        [AllowAnonymous]
        // BỎ [FromBody] ĐỂ ĐỌC RAW BODY
        public async Task<IActionResult> HandleWebhook()
        {
            _logger.LogInformation("PayOS Webhook was hit.");
            string rawBody = string.Empty;

            try
            {
                // 1. ĐỌC RAW BODY TỪ REQUEST
                using (var reader = new StreamReader(Request.Body))
                {
                    rawBody = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(rawBody))
                {
                    _logger.LogWarning("PayOS Webhook: Received an empty body.");
                    return BadRequest("Empty request body.");
                }

                _logger.LogInformation("PayOS Webhook Raw Body: {Body}", rawBody);

                // 2. TỰ PARSE JSON BẰNG NEWTONSOFT
                // (SDK của PayOS dùng Newtonsoft, nên chúng ta dùng nó cho nhất quán)
                var webhookBody = JsonConvert.DeserializeObject<WebhookData>(rawBody);

                if (webhookBody == null)
                {
                    // Đây có thể là request validation của PayOS, nó có thể có cấu trúc khác
                    // Chỉ cần trả về OK là PayOS sẽ hiểu là URL hợp lệ.
                    _logger.LogInformation("PayOS Webhook: Received a request that is not valid WebhookData (possibly a validation ping).");
                    return Ok();
                }

                // 3. TỪ ĐÂY, CODE GIỮ NGUYÊN NHƯ CŨ
                long orderCode = webhookBody.orderCode;
                string orderCodeString = orderCode.ToString();

                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

                if (paymentInfo == null)
                {
                    _logger.LogWarning("PayOS Webhook: Could not get info for orderCode {orderCode}", orderCodeString);
                    return Ok();
                }

                // ... (Phần code còn lại của bạn để xử lý paymentInfo.status, 
                // tìm payment, kiểm tra idempotency, cập nhật DB...)
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "PayOS Webhook: Invalid JSON format. Raw body: {Body}", rawBody);
                return BadRequest(new { message = "Invalid JSON format.", error = jsonEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS Webhook: An unexpected error occurred.");
                // Trả về 200 OK để PayOS không gửi lại, ngay cả khi có lỗi xử lý
                return Ok(new { message = "Error processing, but received." });
            }

            return Ok();
        }
    }
}