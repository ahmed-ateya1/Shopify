using E_Shop.Application.Dtos.ProductImageDto;

namespace E_Shop.Application.ServicesContract
{
    public interface IProductImageService
    {
        Task SaveImagesAsync(ProductImageAddRequest request);
         Task<bool> DeleteImageAsync(Guid ProductId);
    }
}
