using System.ComponentModel.DataAnnotations;

namespace Print3DShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? PrintingServiceId { get; set; }
        public PrintingService? PrintingService { get; set; }

        public int? CustomPrintRequestId { get; set; }
        public CustomPrintRequest? CustomPrintRequest { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}