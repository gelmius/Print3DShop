using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Print3DShop.Data;
using Print3DShop.Models;
using Print3DShop.Services;
using Print3DShop.ViewModels;

namespace Print3DShop.Controllers
{
    public class PrintingServicesController : Controller
    {
        private readonly IPrintingService _printingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ApplicationDbContext _context;

        public PrintingServicesController(
            IPrintingService printingService,
            IShoppingCartService shoppingCartService,
            IFileStorageService fileStorageService,
            ApplicationDbContext context)
        {
            _printingService = printingService;
            _shoppingCartService = shoppingCartService;
            _fileStorageService = fileStorageService;
            _context = context;
        }

        // GET: PrintingServices
        public async Task<IActionResult> Index()
        {
            var services = await _printingService.GetAllPrintingServicesAsync();
            return View(services);
        }

        // GET: PrintingServices/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _printingService.GetPrintingServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: PrintingServices/CustomPrint
        [Authorize]
        public async Task<IActionResult> CustomPrint()
        {
            var services = await _printingService.GetAllPrintingServicesAsync();
            var viewModel = new CustomPrintViewModel
            {
                PrintingServices = services.ToList()
            };
            return View(viewModel);
        }

        // POST: PrintingServices/CustomPrint
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CustomPrint(CustomPrintViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var request = new CustomPrintRequest
                {
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    PrintingServiceId = viewModel.PrintingServiceId
                };

                // Handle model file
                if (viewModel.ModelFile != null)
                {
                    if (_fileStorageService.IsValid3DModelFile(viewModel.ModelFile))
                    {
                        request.ModelFilePath = await _fileStorageService.SaveFileAsync(viewModel.ModelFile, "models/custom");
                    }
                    else
                    {
                        ModelState.AddModelError("ModelFile", "Invalid 3D model file. Allowed formats: stl, obj, 3mf, gcode.");
                        viewModel.PrintingServices = (await _printingService.GetAllPrintingServicesAsync()).ToList();
                        return View(viewModel);
                    }
                }

                // Handle reference image
                if (viewModel.ReferenceImage != null)
                {
                    if (_fileStorageService.IsValidImageFile(viewModel.ReferenceImage))
                    {
                        request.ReferenceImagePath = await _fileStorageService.SaveFileAsync(viewModel.ReferenceImage, "images/custom");
                    }
                    else
                    {
                        ModelState.AddModelError("ReferenceImage", "Invalid image file. Allowed formats: jpg, jpeg, png, gif.");
                        viewModel.PrintingServices = (await _printingService.GetAllPrintingServicesAsync()).ToList();
                        return View(viewModel);
                    }
                }

                // Calculate estimated price if dimensions are provided
                if (viewModel.EstimatedWeightInGrams > 0 && viewModel.EstimatedPrintTimeInHours > 0)
                {
                    request.EstimatedPrice = await _printingService.CalculatePrintingCostAsync(
                        viewModel.PrintingServiceId,
                        viewModel.EstimatedWeightInGrams,
                        viewModel.EstimatedPrintTimeInHours);
                }

                await _printingService.SubmitCustomPrintRequestAsync(request);
                return RedirectToAction("MyRequests");
            }

            viewModel.PrintingServices = (await _printingService.GetAllPrintingServicesAsync()).ToList();
            return View(viewModel);
        }

        // GET: PrintingServices/MyRequests
        [Authorize]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var requests = await _printingService.GetCustomPrintRequestsByUserIdAsync(userId);
            return View(requests);
        }

        // GET: PrintingServices/RequestDetails/5
        [Authorize]
        public async Task<IActionResult> RequestDetails(int id)
        {
            var request = await _printingService.GetCustomPrintRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (request.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(request);
        }

        // POST: PrintingServices/AddToCart/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            await _shoppingCartService.AddToCartAsync(null, id, null, quantity);
            return RedirectToAction("Index", "ShoppingCart");
        }

        // POST: PrintingServices/AddCustomRequestToCart/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCustomRequestToCart(int id)
        {
            var request = await _printingService.GetCustomPrintRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (request.UserId != userId)
            {
                return Forbid();
            }

            if (request.Status != "Approved" || !request.EstimatedPrice.HasValue)
            {
                TempData["ErrorMessage"] = "This request cannot be added to cart. It must be approved and have an estimated price.";
                return RedirectToAction("RequestDetails", new { id });
            }

            await _shoppingCartService.AddToCartAsync(null, null, id, 1);
            return RedirectToAction("Index", "ShoppingCart");
        }

        // GET: PrintingServices/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrintingServices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PrintingServiceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var service = new Models.PrintingService
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    BasePrice = viewModel.BasePrice,
                    PricePerGram = viewModel.PricePerGram,
                    PricePerHour = viewModel.PricePerHour,
                    IsAvailable = viewModel.IsAvailable
                };

                // Handle thumbnail image
                if (viewModel.ThumbnailImage != null)
                {
                    if (_fileStorageService.IsValidImageFile(viewModel.ThumbnailImage))
                    {
                        service.ThumbnailImagePath = await _fileStorageService.SaveFileAsync(viewModel.ThumbnailImage, "images/services");
                    }
                    else
                    {
                        ModelState.AddModelError("ThumbnailImage", "Invalid image file. Allowed formats: jpg, jpeg, png, gif.");
                        return View(viewModel);
                    }
                }

                await _printingService.CreatePrintingServiceAsync(service);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: PrintingServices/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _printingService.GetPrintingServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var viewModel = new PrintingServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                BasePrice = service.BasePrice,
                PricePerGram = service.PricePerGram,
                PricePerHour = service.PricePerHour,
                IsAvailable = service.IsAvailable,
                ExistingThumbnailPath = service.ThumbnailImagePath
            };

            return View(viewModel);
        }

        // POST: PrintingServices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, PrintingServiceViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var service = await _printingService.GetPrintingServiceByIdAsync(id);
                if (service == null)
                {
                    return NotFound();
                }

                service.Name = viewModel.Name;
                service.Description = viewModel.Description;
                service.BasePrice = viewModel.BasePrice;
                service.PricePerGram = viewModel.PricePerGram;
                service.PricePerHour = viewModel.PricePerHour;
                service.IsAvailable = viewModel.IsAvailable;

                // Handle thumbnail image
                if (viewModel.ThumbnailImage != null)
                {
                    if (_fileStorageService.IsValidImageFile(viewModel.ThumbnailImage))
                    {
                        // Delete old thumbnail if exists
                        if (!string.IsNullOrEmpty(service.ThumbnailImagePath))
                        {
                            await _fileStorageService.DeleteFileAsync(service.ThumbnailImagePath);
                        }

                        service.ThumbnailImagePath = await _fileStorageService.SaveFileAsync(viewModel.ThumbnailImage, "images/services");
                    }
                    else
                    {
                        ModelState.AddModelError("ThumbnailImage", "Invalid image file. Allowed formats: jpg, jpeg, png, gif.");
                        return View(viewModel);
                    }
                }

                await _printingService.UpdatePrintingServiceAsync(service);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: PrintingServices/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _printingService.GetPrintingServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: PrintingServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _printingService.GetPrintingServiceByIdAsync(id);
            if (service != null)
            {
                // Delete thumbnail image
                if (!string.IsNullOrEmpty(service.ThumbnailImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(service.ThumbnailImagePath);
                }

                await _printingService.DeletePrintingServiceAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: PrintingServices/ManageRequests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRequests()
        {
            var requests = await _context.CustomPrintRequests
                .Include(r => r.PrintingService)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(requests);
        }

        // GET: PrintingServices/ReviewRequest/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReviewRequest(int id)
        {
            var request = await _printingService.GetCustomPrintRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var viewModel = new ReviewRequestViewModel
            {
                RequestId = request.Id,
                Name = request.Name,
                Description = request.Description,
                ModelFilePath = request.ModelFilePath,
                ReferenceImagePath = request.ReferenceImagePath,
                PrintingServiceId = request.PrintingServiceId,
                PrintingServiceName = request.PrintingService.Name,
                Status = request.Status,
                AdminNotes = request.AdminNotes,
                EstimatedWeightInGrams = 0,
                EstimatedPrintTimeInHours = 0
            };

            if (request.EstimatedPrice.HasValue)
            {
                viewModel.EstimatedPrice = request.EstimatedPrice.Value;
            }

            return View(viewModel);
        }

        // POST: PrintingServices/ReviewRequest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReviewRequest(ReviewRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var request = await _printingService.GetCustomPrintRequestByIdAsync(viewModel.RequestId);
                if (request == null)
                {
                    return NotFound();
                }

                // Calculate estimated price if dimensions are provided
                if (viewModel.EstimatedWeightInGrams > 0 && viewModel.EstimatedPrintTimeInHours > 0)
                {
                    request.EstimatedPrice = await _printingService.CalculatePrintingCostAsync(
                        request.PrintingServiceId,
                        viewModel.EstimatedWeightInGrams,
                        viewModel.EstimatedPrintTimeInHours);
                }
                else if (viewModel.EstimatedPrice > 0)
                {
                    request.EstimatedPrice = viewModel.EstimatedPrice;
                }

                await _printingService.UpdateCustomPrintRequestStatusAsync(
                    viewModel.RequestId,
                    viewModel.Status,
                    viewModel.AdminNotes);

                return RedirectToAction("ManageRequests");
            }

            return View(viewModel);
        }
    }
}