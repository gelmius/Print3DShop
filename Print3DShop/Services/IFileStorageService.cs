using Microsoft.AspNetCore.Http;

namespace Print3DShop.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderPath);
        Task DeleteFileAsync(string filePath);
        bool IsValidImageFile(IFormFile file);
        bool IsValid3DModelFile(IFormFile file);
    }
}