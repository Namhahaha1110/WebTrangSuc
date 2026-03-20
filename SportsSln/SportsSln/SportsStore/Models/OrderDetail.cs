using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }      // PK
        public int OrderID { get; set; }            // FK đến Order
        public Order Order { get; set; } = null!;

        public long ProductID { get; set; }         // FK đến Product
        public Product Product { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
