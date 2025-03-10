using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class ReviewRequestViewModel
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string? ModelFilePath { get; set; }

        public string? ReferenceImagePath { get; set; }

        public int PrintingServiceId { get; set; }

        public string PrintingServiceName { get; set; }

        [Required]
        public string Status { get; set; }

        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }

        [Display(Name = "Estimated Weight (grams)")]
        [Range(0, 10000)]
        public decimal EstimatedWeightInGrams { get; set; }

        [Display(Name = "Estimated Print Time (hours)")]
        [Range(0, 1000)]
        public decimal EstimatedPrintTimeInHours { get; set; }

        [Display(Name = "Estimated Price")]
        [Range(0, 10000)]
        [DataType(DataType.Currency)]
        public decimal EstimatedPrice { get; set; }
    }
}