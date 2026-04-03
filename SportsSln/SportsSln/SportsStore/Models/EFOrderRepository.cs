using Microsoft.EntityFrameworkCore;
using SportsStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly StoreDbContext context;

        public EFOrderRepository(StoreDbContext ctx)
        {
            context = ctx;
        }

        // Lấy danh sách đơn hàng, bao gồm chi tiết và sản phẩm
        public IQueryable<Order> Orders => context.Orders
            .Include(o => o.Lines)
            .ThenInclude(l => l.Product);

        public int SaveOrder(Order order)
        {
            try
            {
                if (order.OrderID == 0)
                {
                    // Thêm đơn hàng mới
                    foreach (var line in order.Lines)
                    {
                        line.Order = order;
                        line.Price = line.Product.Price;
                    }

                    context.Orders.Add(order);
                    context.SaveChanges();

                    context.Entry(order).Reload();
                    context.Entry(order)
                        .Collection(o => o.Lines)
                        .Load();
                    foreach (var line in order.Lines)
                    {
                        context.Entry(line)
                            .Reference(l => l.Product)
                            .Load();
                    }
                }
                else
                {
                    // Cập nhật đơn hàng cũ
                    var existingOrder = context.Orders
                        .Include(o => o.Lines)
                        .ThenInclude(l => l.Product)
                        .FirstOrDefault(o => o.OrderID == order.OrderID);

                    if (existingOrder != null)
                    {
                        // Cập nhật thông tin cơ bản
                        existingOrder.Name = order.Name;
                        existingOrder.City = order.City;
                        existingOrder.Province = order.Province;
                        existingOrder.District = order.District;
                        existingOrder.Ward = order.Ward;
                        existingOrder.AddressDetail = order.AddressDetail;
                        existingOrder.DeliveryType = order.DeliveryType;
                        existingOrder.Store = order.Store;
                        existingOrder.TotalPrice = order.TotalPrice;
                        existingOrder.CreatedDate = order.CreatedDate;

                        // ✅ Chỉ xóa/Thêm CartLine nếu có dữ liệu mới từ frontend
                        if (order.Lines != null && order.Lines.Count > 0)
                        {
                            // Xóa các dòng cũ
                            context.CartLines.RemoveRange(existingOrder.Lines);

                            // Thêm dòng mới
                            existingOrder.Lines = new List<CartLine>();
                            foreach (var line in order.Lines)
                            {
                                line.Order = existingOrder;
                                line.Price = line.Product.Price;
                                existingOrder.Lines.Add(line);
                            }
                        }
                        // Nếu Lines null hoặc rỗng => giữ nguyên chi tiết
                    }

                    context.SaveChanges();
                }

                return order.OrderID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EFOrderRepository] SaveOrder Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[EFOrderRepository] Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public void UpdateOrderShippedStatus(int orderId, bool shipped)
        {
            try
            {
                var order = context.Orders
                    .Include(o => o.Lines)
                    .ThenInclude(l => l.Product)
                    .FirstOrDefault(o => o.OrderID == orderId);

                if (order != null)
                {
                    order.Shipped = shipped;

                    // ⭐ GÁN NGÀY GIAO NẾU ĐÃ GIAO
                    if (shipped)
                        order.ShippedDate = DateTime.Now;
                    else
                        order.ShippedDate = null;   // ⭐ Reset khi chuyển lại trạng thái “Chưa giao”

                    context.SaveChanges();
                    Console.WriteLine($"[Updated] Order {orderId} shipped={shipped}, shippedDate={order.ShippedDate}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EFOrderRepository] UpdateOrderShippedStatus Error: {ex.Message}");
                throw;
            }
        }

        public void UpdateOrderPaymentStatus(int orderId, string paymentStatus)
        {
            try
            {
                var order = context.Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null)
                {
                    return;
                }

                order.Payment = paymentStatus;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EFOrderRepository] UpdateOrderPaymentStatus Error: {ex.Message}");
                throw;
            }
        }

    }
}
