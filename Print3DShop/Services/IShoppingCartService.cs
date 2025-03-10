using Print3DShop.Models;

namespace Print3DShop.Services
{
    public interface IShoppingCartService
    {
        string ShoppingCartId { get; set; }
        Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync();
        Task AddToCartAsync(int? productId, int? printingServiceId, int? customPrintRequestId, int quantity);
        Task RemoveFromCartAsync(int id);
        Task ClearCartAsync();
        Task<int> GetCountAsync();
        Task<decimal> GetTotalAsync();
        Task MigrateCartAsync(string userId);
    }
}