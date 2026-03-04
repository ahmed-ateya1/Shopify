using Microsoft.AspNetCore.Http;

namespace E_Shop.Application.Dtos.ProductDtos
{
    public record ProductAddRequest(string Name, 
        string SKU,
        decimal Price, 
        int StockQuantity, 
        Guid CategoryId,
        IEnumerable<IFormFile> Images,
        bool IsActive = false
        );
}
