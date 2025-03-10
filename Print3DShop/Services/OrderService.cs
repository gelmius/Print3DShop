using Microsoft.EntityFrameworkCore;
using Print3DShop.Data;
using Print3DShop.Models;

namespace Print3DShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _shoppingCartService;

        public OrderService(ApplicationDbContext context, IShoppingCartService shoppingCartService)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<Order> CreateOrderAsync(Order order, string cartId)
        {
            // Set the shopping cart ID to retrieve the items
            _shoppingCartService.ShoppingCartId = cartId;

            // Get the items from the cart
            var cartItems = await _shoppingCartService.GetShoppingCartItemsAsync();

            // Add order items from cart items
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    PrintingServiceId = item.PrintingServiceId,
                    CustomPrintRequestId = item.CustomPrintRequestId,
                    Quantity = item.Quantity
                };

                // Set the unit price based on the item type
                if (item.Product != null)
                {
                    orderItem.UnitPrice = item.Product.Price;
                }
                else if (item.PrintingService != null)
                {
                    orderItem.UnitPrice = item.PrintingService.BasePrice;
                }
                else if (item.CustomPrintRequest != null && item.CustomPrintRequest.EstimatedPrice.HasValue)
                {
                    orderItem.UnitPrice = item.CustomPrintRequest.EstimatedPrice.Value;
                }

                order.OrderItems.Add(orderItem);
            }

            // Calculate the total amount
            order.TotalAmount = order.OrderItems.Sum(item => item.UnitPrice * item.Quantity);

            // Add the order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear the cart
            await _shoppingCartService.ClearCartAsync();

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.PrintingService)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CustomPrintRequest)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;

                // Update shipped or delivered date if applicable
                if (status == "Shipped" && !order.ShippedDate.HasValue)
                {
                    order.ShippedDate = DateTime.Now;
                }
                else if (status == "Delivered" && !order.DeliveredDate.HasValue)
                {
                    order.DeliveredDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTrackingNumberAsync(int id, string trackingNumber)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.TrackingNumber = trackingNumber;
                await _context.SaveChangesAsync();
            }
        }
    }
}