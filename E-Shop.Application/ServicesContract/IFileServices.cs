using Microsoft.AspNetCore.Http;

namespace E_Shop.Application.ServicesContract
{
    public interface IFileServices
    {

        Task<string> CreateFile(IFormFile file);

        Task DeleteFile(string? imageUrl);

        Task<string> UpdateFile(IFormFile newFile, string? currentFileName);
    }
}
