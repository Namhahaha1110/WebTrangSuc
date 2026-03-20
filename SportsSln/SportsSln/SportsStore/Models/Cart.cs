using Microsoft.AspNetCore.Http;
using SportsStore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SportsStore.Models
{
    public class Cart
    {
        private readonly IStoreRepository? repository;

        // Cho phép gộp CartService khi cần
        public Cart ShoppingCart { get; set; }

        public string ReturnUrl { get; set; } = "/";

        // ✅ Thêm 2 property cho QR VietQR
        public string OrderId { get; set; } = Guid.NewGuid().ToString("N"); // mã đơn hàng tạm thời
        public string QrUrl { get; set; } = string.Empty;

        // List sản phẩm trong giỏ
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        // ==================== CONSTRUCTORS ====================
        // Constructor mặc định để SessionCart deserialize được
        public Cart() { }

        // Constructor đầy đủ khi cần repository
        public Cart(IStoreRepository repo, Cart cartService)
        {
            repository = repo;
            ShoppingCart = cartService;
        }

        // ==================== GIỎ HÀNG ====================
        // Thêm sản phẩm
        public virtual void AddItem(Product product, int quantity)
        {
            CartLine? line = Lines.FirstOrDefault(p => p.ProductID == product.ProductID);

            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    ProductID = product.ProductID,
                    Product = product,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        // Xóa sản phẩm
        public virtual void RemoveLine(Product product) =>
            Lines.RemoveAll(l => l.ProductID == product.ProductID);

        // Xóa tất cả
        public virtual void Clear() => Lines.Clear();

        // Cập nhật số lượng
        public virtual void UpdateQuantity(Product product, int quantity)
        {
            CartLine? line = Lines.FirstOrDefault(p => p.ProductID == product.ProductID);
            if (line != null)
            {
                line.Quantity = quantity;
            }
        }

        // Tính tổng tiền
        public decimal ComputeTotalValue() => Lines.Sum(e => e.Price * e.Quantity);

        // Tạo QR VietQR (dùng frontend hoặc OnGet Razor Page)
        public void GenerateQrUrl(decimal total)
        {
            string bankCode = "ACB";
            string accountNumber = "21234611";
            string accountName = "NGUYEN THI YEN NHI";

            QrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact.png" +
                    $"?amount={total}&addInfo={OrderId}&accountName={Uri.EscapeDataString(accountName)}";
        }
    }

    // ==================== CARTLINE ====================
    public class CartLine
    {
        public int CartLineID { get; set; }

        // FK rõ ràng đến Order
        public int? OrderID { get; set; }
        public Order? Order { get; set; }

        // FK đến Product
        public long ProductID { get; set; }
        public Product Product { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }

}
