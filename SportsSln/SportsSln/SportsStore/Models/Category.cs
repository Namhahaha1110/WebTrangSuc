using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models
{
    public class Category
    {
        [Key]
        public long CategoryID { get; set; }

        [Required(ErrorMessage = "Tên phân loại không được để trống")]
        [StringLength(100, ErrorMessage = "Tên phân loại tối đa 100 ký tự")]
        public string? Name { get; set; }

    }
}
