using E_Shop.Application.Dtos.ProductDtos;

namespace E_Shop.Application.ServicesContract
{
    public interface IProductService
    {
        Task<ProductResponse> AddProductAsync(ProductAddRequest request);
        Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest request);

        Task<bool> DeleteProductAsync(Guid productID);
        Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(
            Expression<Func<Product, bool>>? filter = null,
            PaginationDto? pagination = null
            );

        Task<ProductResponse> GetProductByAsync(Expression<Func<Product, bool>> filter);
    }
}
