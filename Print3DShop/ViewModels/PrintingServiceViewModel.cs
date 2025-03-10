using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class PrintingServiceViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }

        [Required]
        [Range(0.01, 1000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Gram")]
        public decimal PricePerGram { get; set; }

        [Required]
        [Range(0.01, 1000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Hour")]
        public decimal PricePerHour { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Thumbnail Image")]
        public IFormFile? ThumbnailImage { get; set; }

        // For edit view
        public string? ExistingThumbnailPath { get; set; }
    }
}