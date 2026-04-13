using SportsStore.Models;

namespace SportsStore.Services
{
    /// <summary>Kết quả sau khi đồng bộ hạng 1 khách hàng.</summary>
    public record TierUpdateResult(
        string UserId,
        string UserName,
        decimal TotalSpent,
        CustomerTier OldTier,
        CustomerTier NewTier,
        bool TierChanged,
        bool IsManual = false
    );

    public interface ICustomerTierService
    {
        /// <summary>Tính toán và cập nhật hạng + TotalSpent cho 1 khách hàng.</summary>
        Task<TierUpdateResult> UpdateCustomerTierAsync(string userId);

        /// <summary>Đồng bộ hạng cho TẤT CẢ khách hàng.</summary>
        Task<List<TierUpdateResult>> SyncAllCustomersAsync();

        /// <summary>Admin tự đặt hạng thủ công (bỏ qua ngưỡng tự động).</summary>
        Task SetTierManuallyAsync(string userId, CustomerTier tier);

        /// <summary>Tính hạng tương ứng với tổng chi tiêu (pure, không DB).</summary>
        CustomerTier CalculateTier(decimal totalSpent);
    }
}
