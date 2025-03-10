using System.ComponentModel.DataAnnotations;

namespace Print3DShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? PrintingServiceId { get; set; }
        public PrintingService? PrintingService { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}