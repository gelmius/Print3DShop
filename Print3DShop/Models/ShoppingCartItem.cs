namespace Print3DShop.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public string CartId { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? PrintingServiceId { get; set; }
        public PrintingService? PrintingService { get; set; }

        public int? CustomPrintRequestId { get; set; }
        public CustomPrintRequest? CustomPrintRequest { get; set; }

        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}