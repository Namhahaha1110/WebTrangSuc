namespace SportsStore.Models
{
    /// <summary>
    /// Hạng khách hàng dựa trên tổng chi tiêu tích lũy (các đơn đã giao).
    /// Bronze  : mặc định
    /// Silver  : ≥ 5.000.000 VNĐ
    /// Gold    : ≥ 15.000.000 VNĐ
    /// Diamond : ≥ 30.000.000 VNĐ
    /// </summary>
    public enum CustomerTier
    {
        Bronze  = 0,
        Silver  = 1,
        Gold    = 2,
        Diamond = 3
    }
}
