using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }

        // ── Phân hạng khách hàng ──────────────────────────────────────
        /// <summary>Hạng hiện tại của khách hàng. Mặc định: Bronze.</summary>
        public CustomerTier Tier { get; set; } = CustomerTier.Bronze;

        /// <summary>
        /// Tổng tiền tích lũy từ các đơn có Shipped=true.
        /// Được cập nhật bởi CustomerTierService (không tính thủ công trên View).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSpent { get; set; } = 0m;
    }
}

