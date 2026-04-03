using System.Linq;

namespace SportsStore.Models
{
    public interface IOrderRepository
    {
        IQueryable<Order> Orders { get; }
        int SaveOrder(Order order);
        void UpdateOrderShippedStatus(int orderId, bool shipped);
        void UpdateOrderPaymentStatus(int orderId, string paymentStatus);
    }
}
