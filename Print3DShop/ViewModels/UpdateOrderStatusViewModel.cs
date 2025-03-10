using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class UpdateOrderStatusViewModel
    {
        public int OrderId { get; set; }

        [Display(Name = "Current Status")]
        public string CurrentStatus { get; set; }

        [Required]
        [Display(Name = "New Status")]
        public string NewStatus { get; set; }

        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }
    }
}