using Microsoft.AspNetCore.Mvc;

namespace VietQRPaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string bankCode = "ACB";
        private readonly string accountNumber = "21234611";
        private readonly string accountName = "NGUYEN THI YEN NHI";

        [HttpGet("create")]
        public IActionResult CreateQR(decimal amount, string orderId)
        {
            if (amount <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Amount must be greater than 0"
                });
            }

            // Tạo URL QR VietQR
            string qrUrl =
                $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact.png" +
                $"?amount={amount}&addInfo={orderId}&accountName={Uri.EscapeDataString(accountName)}";

            return Ok(new
            {
                success = true,
                orderId,
                amount,
                bankCode,
                accountNumber,
                accountName,
                qrUrl
            });
        }
    }
}
