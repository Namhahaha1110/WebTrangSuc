using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models
{
    public class Banner
    {
        public long BannerID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? RedirectUrl { get; set; }

        public int DisplayOrder { get; set; }

        [Range(0, 100)]
        public int FocusX { get; set; } = 50;

        [Range(0, 100)]
        public int FocusY { get; set; } = 50;

        public bool IsActive { get; set; } = true;
    }
}
