namespace Print3DShop.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ImagePath { get; set; }

        public bool IsMain { get; set; }

        public int DisplayOrder { get; set; }
    }
}