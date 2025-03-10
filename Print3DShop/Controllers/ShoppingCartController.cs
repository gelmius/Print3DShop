using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Print3DShop.Services;
using Print3DShop.ViewModels;

namespace Print3DShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IPrintingService _printingService;

        public ShoppingCartController(
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IPrintingService printingService)
        {
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _printingService = printingService;
        }

        // GET: ShoppingCart
        public async Task<IActionResult> Index()
        {
            var items = await _shoppingCartService.GetShoppingCartItemsAsync();
            var total = await _shoppingCartService.GetTotalAsync();

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = items,
                CartTotal = total
            };

            return View(viewModel);
        }

        // POST: ShoppingCart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int? productId, int? printingServiceId, int? customPrintRequestId, int quantity = 1)
        {
            await _shoppingCartService.AddToCartAsync(productId, printingServiceId, customPrintRequestId, quantity);
            return RedirectToAction("Index");
        }

        // POST: ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            await _shoppingCartService.RemoveFromCartAsync(id);
            return RedirectToAction("Index");
        }

        // POST: ShoppingCart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
            {
                await _shoppingCartService.RemoveFromCartAsync(id);
            }
            else
            {
                // Remove the item and add it again with the new quantity
                var items = await _shoppingCartService.GetShoppingCartItemsAsync();
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    await _shoppingCartService.RemoveFromCartAsync(id);
                    await _shoppingCartService.AddToCartAsync(item.ProductId, item.PrintingServiceId, item.CustomPrintRequestId, quantity);
                }
            }
            return RedirectToAction("Index");
        }

        // GET: ShoppingCart/Checkout
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var items = await _shoppingCartService.GetShoppingCartItemsAsync();
            if (!items.Any())
            {
                return RedirectToAction("Index");
            }

            var total = await _shoppingCartService.GetTotalAsync();

            var viewModel = new CheckoutViewModel
            {
                CartItems = items,
                CartTotal = total
            };

            return View(viewModel);
        }
    }
}