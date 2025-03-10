using Microsoft.EntityFrameworkCore;
using Print3DShop.Data;
using Print3DShop.Models;

namespace Print3DShop.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string ShoppingCartId { get; set; }

        public ShoppingCartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            ShoppingCartId = GetCartId();
        }

        private string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            string cartId = session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                session.SetString("CartId", cartId);
            }

            return cartId;
        }

        public async Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync()
        {
            return await _context.ShoppingCartItems
                .Where(c => c.CartId == ShoppingCartId)
                .Include(c => c.Product)
                .Include(c => c.PrintingService)
                .Include(c => c.CustomPrintRequest)
                .ToListAsync();
        }

        public async Task AddToCartAsync(int? productId, int? printingServiceId, int? customPrintRequestId, int quantity)
        {
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.CartId == ShoppingCartId &&
                                         c.ProductId == productId &&
                                         c.PrintingServiceId == printingServiceId &&
                                         c.CustomPrintRequestId == customPrintRequestId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    CartId = ShoppingCartId,
                    ProductId = productId,
                    PrintingServiceId = printingServiceId,
                    CustomPrintRequestId = customPrintRequestId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int id)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var cartItems = await _context.ShoppingCartItems
                .Where(c => c.CartId == ShoppingCartId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.ShoppingCartItems
                .Where(c => c.CartId == ShoppingCartId)
                .SumAsync(c => c.Quantity);
        }

        public async Task<decimal> GetTotalAsync()
        {
            decimal total = 0;
            var cartItems = await GetShoppingCartItemsAsync();

            foreach (var item in cartItems)
            {
                if (item.Product != null)
                {
                    total += item.Product.Price * item.Quantity;
                }
                else if (item.PrintingService != null)
                {
                    total += item.PrintingService.BasePrice * item.Quantity;
                }
                else if (item.CustomPrintRequest != null && item.CustomPrintRequest.EstimatedPrice.HasValue)
                {
                    total += item.CustomPrintRequest.EstimatedPrice.Value * item.Quantity;
                }
            }

            return total;
        }

        public async Task MigrateCartAsync(string userId)
        {
            var cartItems = await GetShoppingCartItemsAsync();
            foreach (var item in cartItems)
            {
                item.CartId = userId;
            }
            await _context.SaveChangesAsync();
        }
    }
}