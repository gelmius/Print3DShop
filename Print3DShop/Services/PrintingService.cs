using Microsoft.EntityFrameworkCore;
using Print3DShop.Data;
using Print3DShop.Models;

namespace Print3DShop.Services
{
    public class PrintingService : IPrintingService
    {
        private readonly ApplicationDbContext _context;

        public PrintingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.PrintingService>> GetAllPrintingServicesAsync()
        {
            return await _context.PrintingServices
                .Include(ps => ps.Reviews)
                .ToListAsync();
        }

        public async Task<Models.PrintingService> GetPrintingServiceByIdAsync(int id)
        {
            return await _context.PrintingServices
                .Include(ps => ps.Reviews)
                .FirstOrDefaultAsync(ps => ps.Id == id);
        }

        public async Task<Models.PrintingService> CreatePrintingServiceAsync(Models.PrintingService printingService)
        {
            _context.PrintingServices.Add(printingService);
            await _context.SaveChangesAsync();
            return printingService;
        }

        public async Task UpdatePrintingServiceAsync(Models.PrintingService printingService)
        {
            _context.Entry(printingService).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePrintingServiceAsync(int id)
        {
            var printingService = await _context.PrintingServices.FindAsync(id);
            if (printingService != null)
            {
                _context.PrintingServices.Remove(printingService);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculatePrintingCostAsync(int printingServiceId, decimal weightInGrams, decimal estimatedHours)
        {
            var printingService = await _context.PrintingServices.FindAsync(printingServiceId);
            if (printingService == null)
                throw new ArgumentException("Invalid printing service ID");

            // Calculate cost based on base price, material weight, and printing time
            decimal materialCost = weightInGrams * printingService.PricePerGram;
            decimal timeCost = estimatedHours * printingService.PricePerHour;
            decimal totalCost = printingService.BasePrice + materialCost + timeCost;

            return Math.Round(totalCost, 2);
        }

        public async Task<CustomPrintRequest> SubmitCustomPrintRequestAsync(CustomPrintRequest request)
        {
            _context.CustomPrintRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<CustomPrintRequest>> GetCustomPrintRequestsByUserIdAsync(string userId)
        {
            return await _context.CustomPrintRequests
                .Where(cpr => cpr.UserId == userId)
                .Include(cpr => cpr.PrintingService)
                .ToListAsync();
        }

        public async Task<CustomPrintRequest> GetCustomPrintRequestByIdAsync(int id)
        {
            return await _context.CustomPrintRequests
                .Include(cpr => cpr.PrintingService)
                .FirstOrDefaultAsync(cpr => cpr.Id == id);
        }

        public async Task UpdateCustomPrintRequestStatusAsync(int id, string status, string adminNotes)
        {
            var request = await _context.CustomPrintRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = status;
                request.AdminNotes = adminNotes;
                request.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Explicit interface implementations to match the return types
        async Task<IEnumerable<Models.PrintingService>> IPrintingService.GetAllPrintingServicesAsync()
        {
            return (IEnumerable<Models.PrintingService>)await GetAllPrintingServicesAsync();
        }

        async Task<Models.PrintingService> IPrintingService.GetPrintingServiceByIdAsync(int id)
        {
            return await GetPrintingServiceByIdAsync(id);
        }

        async Task<Models.PrintingService> IPrintingService.CreatePrintingServiceAsync(Models.PrintingService printingService)
        {
            return await CreatePrintingServiceAsync(printingService);
        }

        async Task IPrintingService.UpdatePrintingServiceAsync(Models.PrintingService printingService)
        {
            await UpdatePrintingServiceAsync(printingService);
        }
    }
}