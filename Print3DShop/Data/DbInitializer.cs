using Microsoft.AspNetCore.Identity;
using Print3DShop.Models;

namespace Print3DShop.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Look for any products
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            // Create roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminUser = new IdentityUser
            {
                UserName = "admin@print3dshop.com",
                Email = "admin@print3dshop.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create categories
            var categories = new Category[]
            {
                new Category { Name = "Figurines", Description = "Decorative 3D printed figurines and statues" },
                new Category { Name = "Home Decor", Description = "3D printed items for home decoration" },
                new Category { Name = "Gadgets", Description = "Useful 3D printed gadgets and tools" },
                new Category { Name = "Jewelry", Description = "3D printed jewelry and accessories" },
                new Category { Name = "Educational", Description = "Educational 3D printed models and tools" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Create products
            var products = new Product[]
            {
                new Product {
                    Name = "Dragon Figurine",
                    Description = "Detailed dragon figurine with textured scales and wings",
                    Price = 29.99m,
                    StockQuantity = 15,
                    CategoryId = categories[0].Id,
                    ThumbnailImagePath = "/images/products/dragon-figurine.jpg"
                },
                new Product {
                    Name = "Geometric Vase",
                    Description = "Modern geometric vase for small plants or flowers",
                    Price = 19.99m,
                    StockQuantity = 20,
                    CategoryId = categories[1].Id,
                    ThumbnailImagePath = "/images/products/geometric-vase.jpg"
                },
                new Product {
                    Name = "Phone Stand",
                    Description = "Adjustable phone stand for desk or bedside",
                    Price = 12.99m,
                    StockQuantity = 30,
                    CategoryId = categories[2].Id,
                    ThumbnailImagePath = "/images/products/phone-stand.jpg"
                },
                new Product {
                    Name = "Customizable Bracelet",
                    Description = "Customizable bracelet with your name or message",
                    Price = 15.99m,
                    StockQuantity = 25,
                    CategoryId = categories[3].Id,
                    ThumbnailImagePath = "/images/products/bracelet.jpg"
                },
                new Product {
                    Name = "Anatomical Heart Model",
                    Description = "Detailed anatomical heart model for educational purposes",
                    Price = 39.99m,
                    StockQuantity = 10,
                    CategoryId = categories[4].Id,
                    ThumbnailImagePath = "/images/products/heart-model.jpg"
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Create printing services
            var printingServices = new PrintingService[]
            {
                new PrintingService {
                    Name = "Standard PLA Printing",
                    Description = "Standard quality 3D printing using PLA material",
                    BasePrice = 10.00m,
                    PricePerGram = 0.15m,
                    PricePerHour = 5.00m,
                    ThumbnailImagePath = "/images/services/pla-printing.jpg"
                },
                new PrintingService {
                    Name = "High Quality PETG Printing",
                    Description = "High quality 3D printing using PETG material for stronger parts",
                    BasePrice = 15.00m,
                    PricePerGram = 0.25m,
                    PricePerHour = 7.50m,
                    ThumbnailImagePath = "/images/services/petg-printing.jpg"
                },
                new PrintingService {
                    Name = "ABS Printing",
                    Description = "3D printing using ABS material for heat-resistant parts",
                    BasePrice = 12.00m,
                    PricePerGram = 0.20m,
                    PricePerHour = 6.00m,
                    ThumbnailImagePath = "/images/services/abs-printing.jpg"
                },
                new PrintingService {
                    Name = "Flexible TPU Printing",
                    Description = "3D printing using flexible TPU material",
                    BasePrice = 18.00m,
                    PricePerGram = 0.30m,
                    PricePerHour = 8.00m,
                    ThumbnailImagePath = "/images/services/tpu-printing.jpg"
                },
                new PrintingService {
                    Name = "Resin Printing",
                    Description = "High detail resin 3D printing for small, detailed objects",
                    BasePrice = 25.00m,
                    PricePerGram = 0.50m,
                    PricePerHour = 10.00m,
                    ThumbnailImagePath = "/images/services/resin-printing.jpg"
                }
            };

            context.PrintingServices.AddRange(printingServices);
            await context.SaveChangesAsync();
        }
    }
}