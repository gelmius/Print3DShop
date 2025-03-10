using Microsoft.AspNetCore.Mvc;
using Print3DShop.Models;
using Print3DShop.Services;
using System.Diagnostics;

namespace Print3DShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IPrintingService _printingService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IPrintingService printingService)
        {
            _logger = logger;
            _productService = productService;
            _printingService = printingService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.GetAllProductsAsync();
            var printingServices = await _printingService.GetAllPrintingServicesAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts.Take(4).ToList(),
                PrintingServices = printingServices.Take(3).ToList()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; }
        public List<Models.PrintingService> PrintingServices { get; set; }
    }
}