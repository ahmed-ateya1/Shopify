using Microsoft.AspNetCore.Http;

namespace E_Shop.Application.Dtos.ProductImageDto
{
    public record ProductImageAddRequest(IEnumerable<IFormFile> Images, Guid ProductId);
}
