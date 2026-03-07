using E_Shop.Application.Dtos.ProductDtos;
using E_Shop.Application.Dtos.ProductImageDto;
using E_Shop.Application.Exceptions;
using Mapster;

namespace E_Shop.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork,
        IProductImageService imageService,
        ILogger<ProductService> logger)
        : IProductService
    {
        private async Task ExecuteWithTransactionAsync(Func<Task> action)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Transaction failed and rolled back.");
                throw;
            }
        }
        public async Task<ProductResponse> AddProductAsync(ProductAddRequest request)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));


            var category = await unitOfWork.Repository<Category>()
                .GetByAsync(x=>x.Id == request.CategoryId);

            if (category == null)
                throw new CategoryNotFound("Category Not Found");

            var productSkuExists = await unitOfWork.Repository<Product>()
                .GetByAsync(x => x.SKU == request.SKU);

            if (productSkuExists != null)
            {
                throw new DuplicateSKUException($"A product with SKU '{request.SKU}' already exists.");
            }

            var product = request.Adapt<Product>();

            await ExecuteWithTransactionAsync(async () =>
            {
                var result = await unitOfWork.Repository<Product>().CreateAsync(product);

                await imageService.SaveImagesAsync(new ProductImageAddRequest(request.Images, result.Id));

            });

            return product.Adapt<ProductResponse>();

        }

        public async Task<bool> DeleteProductAsync(Guid productID)
        {
           var product = await unitOfWork.Repository<Product>()
                .GetByAsync(x => x.Id == productID, includeProperties: "Images");
            if (product == null)
                throw new ProductNotFound("Product Not Found");
            
            await ExecuteWithTransactionAsync(async () =>
            {
                await imageService.DeleteImageAsync(product.Id);
                await unitOfWork.Repository<Product>().DeleteAsync(product);
            });

            return true;
        }

        public async Task<PaginatedResponse<ProductResponse>> GetAllProductsAsync(Expression<Func<Product, bool>>? filter = null, PaginationDto? pagination = null)
        {
            pagination ??= new PaginationDto { PageIndex = 1, PageSize = 10, SortBy = "CreatedAt", SortDirection = "desc" };

            var products = await unitOfWork.Repository<Product>()
                .GetAllAsync(filter, includeProperties: "Images",
                pagination.SortBy,
                pagination.SortDirection
                ,pagination.PageIndex,
                pagination.PageSize);

            var totalCount = await unitOfWork.Repository<Product>().CountAsync(filter);

            if(products == null || !products.Any())
                return new PaginatedResponse<ProductResponse>();

            var productResponse = products.Adapt<IEnumerable<ProductResponse>>();

            return new PaginatedResponse<ProductResponse>
            {
                Items = productResponse,
                TotalCount = totalCount,
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize
            };
        }

        public async Task<ProductResponse> GetProductByAsync(Expression<Func<Product, bool>> filter)
        {
            var product = await unitOfWork.Repository<Product>()
                .GetByAsync(filter, includeProperties: "Images");

            if (product == null)
                throw new ProductNotFound("Product Not Found");

            return product.Adapt<ProductResponse>();

        }

        public async Task<ProductResponse> UpdateProductAsync(ProductUpdateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var product = await unitOfWork.Repository<Product>()
                .GetByAsync(x => x.Id == request.Id, includeProperties: "Images");

            if (product == null)
                throw new ProductNotFound("Product Not Found");


            var category = await unitOfWork.Repository<Category>()
               .GetByAsync(x => x.Id == request.CategoryId);

            if (category == null)
                throw new CategoryNotFound("Category Not Found");

            var productSkuExists = await unitOfWork.Repository<Product>()
                                    .GetByAsync(x => x.SKU == request.SKU && x.Id != request.Id);

            if (productSkuExists != null)
            {
                throw new DuplicateSKUException($"A product with SKU '{request.SKU}' already exists.");
            }

            await ExecuteWithTransactionAsync(async () =>
            {
                if (request.Images != null && request.Images.Any())
                {
                    await imageService.DeleteImageAsync(product.Id);

                    await imageService.SaveImagesAsync(
                        new ProductImageAddRequest(request.Images, product.Id));
                }

                request.Adapt(product);

                await unitOfWork.Repository<Product>().UpdateAsync(product);
            });

            return product.Adapt<ProductResponse>();
        }
    }
}
