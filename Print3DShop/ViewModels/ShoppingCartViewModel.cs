using Print3DShop.Models;

namespace Print3DShop.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}