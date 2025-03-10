using Microsoft.AspNetCore.Http;
using Print3DShop.Models;
using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class ProductViewModel
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
        public decimal Price { get; set; }

        [Required]
        [Range(0, 1000)]
        public int StockQuantity { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Thumbnail Image")]
        public IFormFile? ThumbnailImage { get; set; }

        [Display(Name = "3D Model File")]
        public IFormFile? ModelFile { get; set; }

        [Display(Name = "Additional Images")]
        public List<IFormFile>? AdditionalImages { get; set; }

        // For edit view
        public string? ExistingThumbnailPath { get; set; }
        public string? ExistingModelFilePath { get; set; }
        public List<ProductImage>? ExistingImages { get; set; }
        public List<int>? ImagesToDelete { get; set; }
    }
}