using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Print3DShop.Data;
using Print3DShop.Models;
using Print3DShop.Services;
using Print3DShop.ViewModels;

namespace Print3DShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ApplicationDbContext _context;

        public ProductsController(
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IFileStorageService fileStorageService,
            ApplicationDbContext context)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _fileStorageService = fileStorageService;
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? categoryId, string searchTerm)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = await _productService.SearchProductsAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }
            else if (categoryId.HasValue)
            {
                products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                ViewBag.CategoryId = categoryId;
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            // Get categories for the filter dropdown
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/AddToCart/5
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            await _shoppingCartService.AddToCartAsync(id, null, null, quantity);
            return RedirectToAction("Index", "ShoppingCart");
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    CategoryId = viewModel.CategoryId,
                    IsAvailable = viewModel.IsAvailable
                };

                // Handle thumbnail image
                if (viewModel.ThumbnailImage != null)
                {
                    if (_fileStorageService.IsValidImageFile(viewModel.ThumbnailImage))
                    {
                        product.ThumbnailImagePath = await _fileStorageService.SaveFileAsync(viewModel.ThumbnailImage, "images/products");
                    }
                    else
                    {
                        ModelState.AddModelError("ThumbnailImage", "Invalid image file. Allowed formats: jpg, jpeg, png, gif.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                        return View(viewModel);
                    }
                }

                // Handle 3D model file
                if (viewModel.ModelFile != null)
                {
                    if (_fileStorageService.IsValid3DModelFile(viewModel.ModelFile))
                    {
                        product.ModelFilePath = await _fileStorageService.SaveFileAsync(viewModel.ModelFile, "models");
                    }
                    else
                    {
                        ModelState.AddModelError("ModelFile", "Invalid 3D model file. Allowed formats: stl, obj, 3mf, gcode.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                        return View(viewModel);
                    }
                }

                await _productService.CreateProductAsync(product);

                // Handle additional images
                if (viewModel.AdditionalImages != null && viewModel.AdditionalImages.Count > 0)
                {
                    foreach (var image in viewModel.AdditionalImages)
                    {
                        if (_fileStorageService.IsValidImageFile(image))
                        {
                            var imagePath = await _fileStorageService.SaveFileAsync(image, "images/products");
                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImagePath = imagePath,
                                IsMain = false
                            };
                            _context.ProductImages.Add(productImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(viewModel);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsAvailable = product.IsAvailable,
                ExistingThumbnailPath = product.ThumbnailImagePath,
                ExistingModelFilePath = product.ModelFilePath,
                ExistingImages = product.Images.ToList()
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.Price = viewModel.Price;
                product.StockQuantity = viewModel.StockQuantity;
                product.CategoryId = viewModel.CategoryId;
                product.IsAvailable = viewModel.IsAvailable;

                // Handle thumbnail image
                if (viewModel.ThumbnailImage != null)
                {
                    if (_fileStorageService.IsValidImageFile(viewModel.ThumbnailImage))
                    {
                        // Delete old thumbnail if exists
                        if (!string.IsNullOrEmpty(product.ThumbnailImagePath))
                        {
                            await _fileStorageService.DeleteFileAsync(product.ThumbnailImagePath);
                        }

                        product.ThumbnailImagePath = await _fileStorageService.SaveFileAsync(viewModel.ThumbnailImage, "images/products");
                    }
                    else
                    {
                        ModelState.AddModelError("ThumbnailImage", "Invalid image file. Allowed formats: jpg, jpeg, png, gif.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                        return View(viewModel);
                    }
                }

                // Handle 3D model file
                if (viewModel.ModelFile != null)
                {
                    if (_fileStorageService.IsValid3DModelFile(viewModel.ModelFile))
                    {
                        // Delete old model file if exists
                        if (!string.IsNullOrEmpty(product.ModelFilePath))
                        {
                            await _fileStorageService.DeleteFileAsync(product.ModelFilePath);
                        }

                        product.ModelFilePath = await _fileStorageService.SaveFileAsync(viewModel.ModelFile, "models");
                    }
                    else
                    {
                        ModelState.AddModelError("ModelFile", "Invalid 3D model file. Allowed formats: stl, obj, 3mf, gcode.");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                        return View(viewModel);
                    }
                }

                await _productService.UpdateProductAsync(product);

                // Handle additional images
                if (viewModel.AdditionalImages != null && viewModel.AdditionalImages.Count > 0)
                {
                    foreach (var image in viewModel.AdditionalImages)
                    {
                        if (_fileStorageService.IsValidImageFile(image))
                        {
                            var imagePath = await _fileStorageService.SaveFileAsync(image, "images/products");
                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImagePath = imagePath,
                                IsMain = false
                            };
                            _context.ProductImages.Add(productImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Handle image deletions
                if (viewModel.ImagesToDelete != null && viewModel.ImagesToDelete.Count > 0)
                {
                    foreach (var imageId in viewModel.ImagesToDelete)
                    {
                        var image = await _context.ProductImages.FindAsync(imageId);
                        if (image != null)
                        {
                            await _fileStorageService.DeleteFileAsync(image.ImagePath);
                            _context.ProductImages.Remove(image);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(viewModel);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                // Delete thumbnail image
                if (!string.IsNullOrEmpty(product.ThumbnailImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(product.ThumbnailImagePath);
                }

                // Delete model file
                if (!string.IsNullOrEmpty(product.ModelFilePath))
                {
                    await _fileStorageService.DeleteFileAsync(product.ModelFilePath);
                }

                // Delete additional images
                foreach (var image in product.Images)
                {
                    await _fileStorageService.DeleteFileAsync(image.ImagePath);
                }

                await _productService.DeleteProductAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}