using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public string UserId { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        // Địa chỉ giao hàng
        [StringLength(50)]
        public string? Province { get; set; }
        [StringLength(50)]
        public string? District { get; set; }
        [StringLength(50)]
        public string? Ward { get; set; }
        [StringLength(200)]
        public string? AddressDetail { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ShippedDate { get; set; }


        // Nhận tại cửa hàng
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(50)]
        public string? DistrictPickup { get; set; }
        [StringLength(100)]
        public string? Store { get; set; }

        [StringLength(20)]
        public string? DeliveryType { get; set; } // "delivery" hoặc "pickup"

        [StringLength(20)]
        public string? Payment { get; set; } // "COD" hoặc "Bank"

        [StringLength(500)]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public bool Shipped { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Quan hệ 1-N: Order → CartLine
        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();


    }
}
