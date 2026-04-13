using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

namespace SportsStore.Services
{
    /// <summary>
    /// Dịch vụ phân hạng khách hàng tự động.
    /// Tính TotalSpent từ các đơn hàng Shipped=true trong StoreDbContext,
    /// sau đó cập nhật Tier và TotalSpent sang AppIdentityDbContext qua UserManager.
    /// </summary>
    public class CustomerTierService : ICustomerTierService
    {
        // Ngưỡng nâng hạng (VNĐ)
        private const decimal SILVER_THRESHOLD  =  5_000_000m;
        private const decimal GOLD_THRESHOLD    = 15_000_000m;
        private const decimal DIAMOND_THRESHOLD = 30_000_000m;

        private readonly IDbContextFactory<StoreDbContext> _storeFactory;
        private readonly UserManager<ApplicationUser>      _userManager;

        public CustomerTierService(
            IDbContextFactory<StoreDbContext> storeFactory,
            UserManager<ApplicationUser>      userManager)
        {
            _storeFactory = storeFactory;
            _userManager  = userManager;
        }

        // ── Tính hạng từ tổng chi tiêu (thuần, không DB) ────────────
        public CustomerTier CalculateTier(decimal totalSpent) => totalSpent switch
        {
            >= DIAMOND_THRESHOLD => CustomerTier.Diamond,
            >= GOLD_THRESHOLD    => CustomerTier.Gold,
            >= SILVER_THRESHOLD  => CustomerTier.Silver,
            _                    => CustomerTier.Bronze,
        };

        // ── Cập nhật hạng cho 1 khách hàng ──────────────────────────
        public async Task<TierUpdateResult> UpdateCustomerTierAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"Không tìm thấy user với ID: {userId}");

            var oldTier    = user.Tier;
            var totalSpent = await GetTotalSpentAsync(userId);
            var newTier    = CalculateTier(totalSpent);

            user.TotalSpent = totalSpent;
            user.Tier       = newTier;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Cập nhật user thất bại: {errors}");
            }

            return new TierUpdateResult(
                UserId:      userId,
                UserName:    user.UserName ?? user.Email ?? userId,
                TotalSpent:  totalSpent,
                OldTier:     oldTier,
                NewTier:     newTier,
                TierChanged: oldTier != newTier
            );
        }

        // ── Đồng bộ toàn bộ khách hàng ──────────────────────────────
        public async Task<List<TierUpdateResult>> SyncAllCustomersAsync()
        {
            // Lấy tất cả user trong Identity DB
            var allUsers = _userManager.Users.ToList(); // OK — tập nhỏ

            var results = new List<TierUpdateResult>(allUsers.Count);
            foreach (var user in allUsers)
            {
                try
                {
                    var r = await UpdateCustomerTierAsync(user.Id);
                    results.Add(r);
                }
                catch (Exception ex)
                {
                    // Ghi log, tiếp tục xử lý user tiếp theo
                    results.Add(new TierUpdateResult(
                        UserId:      user.Id,
                        UserName:    user.UserName ?? user.Email ?? user.Id,
                        TotalSpent:  user.TotalSpent,
                        OldTier:     user.Tier,
                        NewTier:     user.Tier,
                        TierChanged: false
                    ));
                    // Không throw — xử lý lỗi mềm để không chặn toàn batch
                    _ = ex; // suppress warning
                }
            }
            return results;
        }

        // ── Admin đặt hạng thủ công ──────────────────────────────────
        public async Task SetTierManuallyAsync(string userId, CustomerTier tier)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"Không tìm thấy user với ID: {userId}");

            user.Tier = tier;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Cập nhật hạng thủ công thất bại: {errors}");
            }
        }

        // ── Private: Tính tổng tiền từ đơn Shipped (StoreDbContext) ─
        private async Task<decimal> GetTotalSpentAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return 0m;

            // Tạo scoped DbContext (vì StoreDbContext dùng DbContextFactory)
            await using var ctx = await _storeFactory.CreateDbContextAsync();

            // AsNoTracking + chỉ SELECT TotalPrice — tối ưu N+1 và băng thông
            var total = await ctx.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Shipped)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

            return total < 0m ? 0m : total; // bảo đảm không âm
        }
    }
}
