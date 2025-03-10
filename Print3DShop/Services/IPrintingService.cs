using Print3DShop.Models;

namespace Print3DShop.Services
{
    public interface IPrintingService
    {
        Task<IEnumerable<Models.PrintingService>> GetAllPrintingServicesAsync();
        Task<Models.PrintingService> GetPrintingServiceByIdAsync(int id);
        Task<Models.PrintingService> CreatePrintingServiceAsync(Models.PrintingService printingService);
        Task UpdatePrintingServiceAsync(Models.PrintingService printingService);
        Task DeletePrintingServiceAsync(int id);
        Task<decimal> CalculatePrintingCostAsync(int printingServiceId, decimal weightInGrams, decimal estimatedHours);
        Task<CustomPrintRequest> SubmitCustomPrintRequestAsync(CustomPrintRequest request);
        Task<IEnumerable<CustomPrintRequest>> GetCustomPrintRequestsByUserIdAsync(string userId);
        Task<CustomPrintRequest> GetCustomPrintRequestByIdAsync(int id);
        Task UpdateCustomPrintRequestStatusAsync(int id, string status, string adminNotes);
    }
}