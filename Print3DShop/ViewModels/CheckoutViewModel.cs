using Print3DShop.Models;
using System.ComponentModel.DataAnnotations;

namespace Print3DShop.ViewModels
{
    public class CheckoutViewModel
    {
        public List<ShoppingCartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "CreditCard";

        [CreditCard]
        [Display(Name = "Credit Card Number")]
        public string CreditCardNumber { get; set; }

        [Display(Name = "Expiration Date")]
        public string ExpirationDate { get; set; }

        [StringLength(4)]
        [Display(Name = "CVV")]
        public string CVV { get; set; }
    }
}