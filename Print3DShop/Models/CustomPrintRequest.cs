using System.ComponentModel.DataAnnotations;

namespace Print3DShop.Models
{
    public class CustomPrintRequest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ModelFilePath { get; set; }

        public string? ReferenceImagePath { get; set; }

        [Required]
        public int PrintingServiceId { get; set; }
        public PrintingService PrintingService { get; set; }

        [Range(0.01, 10000)]
        public decimal? EstimatedPrice { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        public string? AdminNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}