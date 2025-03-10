using Microsoft.AspNetCore.Mvc;
using Print3DShop.Services;

namespace Print3DShop.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartSummaryViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _shoppingCartService.GetCountAsync();
            return View(count);
        }
    }
}