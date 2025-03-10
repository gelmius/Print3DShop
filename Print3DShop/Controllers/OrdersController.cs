using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Print3DShop.Models;
using Print3DShop.Services;
using Print3DShop.ViewModels;

namespace Print3DShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;

        public OrdersController(IOrderService orderService, IShoppingCartService shoppingCartService)
        {
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
        }

        // POST: Orders/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                var cartId = _shoppingCartService.ShoppingCartId;

                var order = new Order
                {
                    UserId = userId,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    State = viewModel.State,
                    ZipCode = viewModel.ZipCode,
                    Country = viewModel.Country,
                    PaymentTransactionId = Guid.NewGuid().ToString() // In a real app, this would come from a payment gateway
                };

                await _orderService.CreateOrderAsync(order, cartId);

                return RedirectToAction("OrderConfirmation", new { id = order.Id });
            }

            // If we got this far, something failed, redisplay form
            viewModel.CartItems = await _shoppingCartService.GetShoppingCartItemsAsync();
            viewModel.CartTotal = await _shoppingCartService.GetTotalAsync();
            return View("~/Views/ShoppingCart/Checkout.cshtml", viewModel);
        }

        // GET: Orders/OrderConfirmation/5
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: Orders/MyOrders
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: Orders/ManageOrders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // GET: Orders/UpdateStatus/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateOrderStatusViewModel
            {
                OrderId = order.Id,
                CurrentStatus = order.Status,
                TrackingNumber = order.TrackingNumber
            };

            return View(viewModel);
        }

        // POST: Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderStatusAsync(viewModel.OrderId, viewModel.NewStatus);

                if (!string.IsNullOrEmpty(viewModel.TrackingNumber))
                {
                    await _orderService.UpdateTrackingNumberAsync(viewModel.OrderId, viewModel.TrackingNumber);
                }

                return RedirectToAction("ManageOrders");
            }

            return View(viewModel);
        }
    }
}