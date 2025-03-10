using System.ComponentModel.DataAnnotations;

namespace Print3DShop.Models
{
    public class PrintingService
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
        public decimal BasePrice { get; set; }

        // Price per gram of material
        [Range(0.01, 1000)]
        public decimal PricePerGram { get; set; }

        // Price per hour of printing
        [Range(0.01, 1000)]
        public decimal PricePerHour { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ThumbnailImagePath { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [Display(Name = "Average Rating")]
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
    }
}