using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System.Net;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SportsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IConfiguration configuration;

        public PaymentController(IOrderRepository orderRepository, IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.configuration = configuration;
        }

        [HttpGet("vnpay/create-url")]
        public IActionResult CreateVnpayPaymentUrl(int orderId)
        {
            var vnpayConfig = GetVnpayConfig();
            if (!vnpayConfig.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Thiếu cấu hình VNPAY. Vui lòng kiểm tra appsettings (TmnCode, HashSecret, CallbackUrl)."
                });
            }

            var order = orderRepository.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            if (order.TotalPrice < 5000M)
            {
                return BadRequest(new { success = false, message = "Số tiền tối thiểu để thanh toán VNPAY là 5.000 VND" });
            }

            var txnRef = BuildTxnRef(order.OrderID);
            var amount = Convert.ToInt64(order.TotalPrice * 100M);
            var createDate = DateTime.Now;
            var expireDate = createDate.AddMinutes(15);

            var inputData = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = vnpayConfig.Version,
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = vnpayConfig.TmnCode,
                ["vnp_Amount"] = amount.ToString(CultureInfo.InvariantCulture),
                ["vnp_CreateDate"] = createDate.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = $"Thanh toan don hang ORDER-{order.OrderID}",
                ["vnp_OrderType"] = vnpayConfig.OrderType,
                ["vnp_ReturnUrl"] = vnpayConfig.CallbackUrl,
                ["vnp_TxnRef"] = txnRef,
                ["vnp_ExpireDate"] = expireDate.ToString("yyyyMMddHHmmss")
            };

            var paymentUrl = BuildVnpayUrl(vnpayConfig.BaseUrl, inputData, vnpayConfig.HashSecret);

            return Ok(new
            {
                success = true,
                orderId = order.OrderID,
                paymentUrl,
                txnRef,
                amount = order.TotalPrice
            });
        }

        [HttpGet("status")]
        public IActionResult GetStatus(string orderId)
        {
            if (!TryExtractOrderId(orderId, out var numericOrderId))
            {
                return BadRequest(new { success = false, message = "OrderId không hợp lệ" });
            }

            var order = orderRepository.Orders.FirstOrDefault(o => o.OrderID == numericOrderId);
            if (order == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            var isPaid = order.Payment == "VNPAY_PAID" || order.Payment == "BANK_PAID";
            return Ok(new
            {
                success = true,
                orderId = order.OrderID,
                paymentStatus = order.Payment,
                isPaid
            });
        }

        [HttpGet("vnpay-return")]
        public IActionResult VnpayReturn()
        {
            var vnpayConfig = GetVnpayConfig();
            if (!vnpayConfig.IsValid)
            {
                return Redirect("/customer/my-orders?paymentStatus=config-invalid");
            }

            var queryData = Request.Query
                .Where(kv => kv.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            if (!ValidateSignature(queryData, vnpayConfig.HashSecret))
            {
                return Redirect("/customer/my-orders?paymentStatus=invalid-signature");
            }

            var txnRef = queryData.TryGetValue("vnp_TxnRef", out var refValue) ? refValue : string.Empty;
            if (!TryExtractOrderId(txnRef, out var numericOrderId))
            {
                return Redirect("/customer/my-orders?paymentStatus=invalid-order");
            }

            var order = orderRepository.Orders.FirstOrDefault(o => o.OrderID == numericOrderId);
            if (order == null)
            {
                return Redirect("/customer/my-orders?paymentStatus=order-not-found");
            }

            var responseCode = queryData.TryGetValue("vnp_ResponseCode", out var responseValue)
                ? responseValue
                : string.Empty;
            var transactionStatus = queryData.TryGetValue("vnp_TransactionStatus", out var statusValue)
                ? statusValue
                : string.Empty;

            var isSuccess = responseCode == "00" && transactionStatus == "00";
            if (isSuccess)
            {
                if (order.Payment != "VNPAY_PAID")
                {
                    orderRepository.UpdateOrderPaymentStatus(order.OrderID, "VNPAY_PAID");
                }

                return Redirect($"/Completed?orderId={order.OrderID}");
            }

            return Redirect($"/PaymentPending?orderId={order.OrderID}&status=failed&code={responseCode}");
        }

        [HttpGet("vnpay-ipn")]
        public IActionResult VnpayIpn()
        {
            if (Request.Query == null || Request.Query.Count == 0)
            {
                return IpnResponse("99", "Invalid request");
            }

            var vnpayConfig = GetVnpayConfig();
            if (!vnpayConfig.IsValid)
            {
                return IpnResponse("99", "Config invalid");
            }

            var queryData = Request.Query
                .Where(kv => kv.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            if (!ValidateSignature(queryData, vnpayConfig.HashSecret))
            {
                var responseCode = Request.Query["vnp_ResponseCode"].ToString();
                var transactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

                if (responseCode != "00" || transactionStatus != "00")
                {
                    return IpnResponse("00", "Confirm Success");
                }

                return IpnResponse("97", "Invalid checksum");
            }

            var txnRef = queryData.TryGetValue("vnp_TxnRef", out var txnValue) ? txnValue : string.Empty;
            if (!TryExtractOrderId(txnRef, out var numericOrderId))
            {
                return IpnResponse("01", "Order not found");
            }

            var order = orderRepository.Orders.FirstOrDefault(o => o.OrderID == numericOrderId);
            if (order == null)
            {
                return IpnResponse("01", "Order not found");
            }

            var amountValid = queryData.TryGetValue("vnp_Amount", out var amountText)
                && long.TryParse(amountText, out var callbackAmount)
                && callbackAmount == Convert.ToInt64(order.TotalPrice * 100M);

            if (!amountValid)
            {
                return IpnResponse("04", "Invalid amount");
            }

            var responseCodeSuccess = queryData.TryGetValue("vnp_ResponseCode", out var responseValue)
                && responseValue == "00";
            var transactionStatusSuccess = queryData.TryGetValue("vnp_TransactionStatus", out var statusValue)
                && statusValue == "00";

            if (!responseCodeSuccess || !transactionStatusSuccess)
            {
                return IpnResponse("00", "Confirm Success");
            }

            if (order.Payment == "VNPAY_PAID" || order.Payment == "BANK_PAID")
            {
                return IpnResponse("02", "Order already confirmed");
            }

            orderRepository.UpdateOrderPaymentStatus(order.OrderID, "VNPAY_PAID");
            return IpnResponse("00", "Confirm Success");
        }

        [HttpGet("vnpay/config-check")]
        public IActionResult CheckConfig()
        {
            var vnpayConfig = GetVnpayConfig();

            return Ok(new
            {
                success = vnpayConfig.IsValid,
                baseUrl = vnpayConfig.BaseUrl,
                callbackUrl = vnpayConfig.CallbackUrl,
                hasTmnCode = !string.IsNullOrWhiteSpace(vnpayConfig.TmnCode),
                hasHashSecret = !string.IsNullOrWhiteSpace(vnpayConfig.HashSecret)
            });
        }

        private VnpayConfig GetVnpayConfig()
        {
            var section = configuration.GetSection("VNPAY");
            var tmnCode = section["TmnCode"] ?? string.Empty;
            var hashSecret = section["HashSecret"] ?? string.Empty;
            var baseUrl = section["BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var callbackUrl = section["CallbackUrl"] ?? string.Empty;
            var version = section["Version"] ?? "2.1.0";
            var orderType = section["OrderType"] ?? "other";

            return new VnpayConfig
            {
                TmnCode = tmnCode,
                HashSecret = hashSecret,
                BaseUrl = baseUrl,
                CallbackUrl = callbackUrl,
                Version = version,
                OrderType = orderType,
                IsValid = !string.IsNullOrWhiteSpace(tmnCode)
                          && !string.IsNullOrWhiteSpace(hashSecret)
                          && !string.IsNullOrWhiteSpace(callbackUrl)
            };
        }

        private static string BuildVnpayUrl(string baseUrl, SortedDictionary<string, string> inputData, string hashSecret)
        {
            var queryBuilder = new StringBuilder();
            var hashDataBuilder = new StringBuilder();

            foreach (var item in inputData.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)))
            {
                if (queryBuilder.Length > 0)
                {
                    queryBuilder.Append('&');
                    hashDataBuilder.Append('&');
                }

                var encodedKey = WebUtility.UrlEncode(item.Key);
                var encodedValue = WebUtility.UrlEncode(item.Value);

                queryBuilder
                    .Append(encodedKey)
                    .Append('=')
                    .Append(encodedValue);

                hashDataBuilder
                    .Append(encodedKey)
                    .Append('=')
                    .Append(encodedValue);
            }

            var signData = hashDataBuilder.ToString();
            var secureHash = ComputeHmacSha512(hashSecret, signData);
            return $"{baseUrl}?{queryBuilder}&vnp_SecureHashType=HMACSHA512&vnp_SecureHash={secureHash}";
        }

        private static bool ValidateSignature(IDictionary<string, string> queryData, string hashSecret)
        {
            var receivedHash = queryData.TryGetValue("vnp_SecureHash", out var secureHash)
                ? secureHash
                : string.Empty;
            if (string.IsNullOrWhiteSpace(receivedHash))
            {
                return false;
            }

            var payload = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var pair in queryData)
            {
                if (!pair.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase)
                    || pair.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase)
                    || pair.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }

                payload[pair.Key] = pair.Value;
            }

            var signData = string.Join("&", payload.Select(kv =>
                $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var computedHash = ComputeHmacSha512(hashSecret, signData);
            return string.Equals(receivedHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHmacSha512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using var hmac = new HMACSHA512(keyBytes);
            var hashValue = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hashValue).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static bool TryExtractOrderId(string rawOrderId, out int numericOrderId)
        {
            numericOrderId = 0;
            if (string.IsNullOrWhiteSpace(rawOrderId))
            {
                return false;
            }

            var normalized = rawOrderId.Trim();

            // New format: OD{orderId}T{yyMMddHHmmssfff}
            if (normalized.StartsWith("OD", StringComparison.OrdinalIgnoreCase))
            {
                var tIndex = normalized.IndexOf('T');
                if (tIndex > 2)
                {
                    var idPart = normalized.Substring(2, tIndex - 2);
                    return int.TryParse(idPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericOrderId);
                }
            }

            if (normalized.StartsWith("ORDER-", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring("ORDER-".Length);
            }

            return int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericOrderId);
        }

        private static string BuildTxnRef(int orderId)
        {
            // Ensure each payment attempt has a unique reference in VNPAY.
            return $"OD{orderId}T{DateTime.Now:yyMMddHHmmssfff}";
        }

        private JsonResult IpnResponse(string rspCode, string message)
        {
            return new JsonResult(new Dictionary<string, string>
            {
                ["RspCode"] = rspCode,
                ["Message"] = message
            });
        }

        private sealed class VnpayConfig
        {
            public string TmnCode { get; set; } = string.Empty;
            public string HashSecret { get; set; } = string.Empty;
            public string BaseUrl { get; set; } = string.Empty;
            public string CallbackUrl { get; set; } = string.Empty;
            public string Version { get; set; } = "2.1.0";
            public string OrderType { get; set; } = "other";
            public bool IsValid { get; set; }
        }
    }
}
