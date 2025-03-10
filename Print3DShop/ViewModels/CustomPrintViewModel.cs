using Microsoft.AspNetCore.Http;
using Print3DShop.Models;
using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class CustomPrintViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Printing Service")]
        public int PrintingServiceId { get; set; }

        [Required]
        [Display(Name = "3D Model File")]
        public IFormFile ModelFile { get; set; }

        [Display(Name = "Reference Image (Optional)")]
        public IFormFile? ReferenceImage { get; set; }

        [Display(Name = "Estimated Weight (grams)")]
        [Range(0.1, 10000)]
        public decimal EstimatedWeightInGrams { get; set; }

        [Display(Name = "Estimated Print Time (hours)")]
        [Range(0.1, 1000)]
        public decimal EstimatedPrintTimeInHours { get; set; }

        public List<PrintingService> PrintingServices { get; set; }
    }
}