using E_Shop.Application.Dtos.ProductImageDto;

namespace E_Shop.Application.Services
{
    public class ProductImageService(IUnitOfWork unitOfWork,
        IFileServices fileServices,
        ILogger<ProductImageService> logger)
        : IProductImageService
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
        public async Task<bool> DeleteImageAsync(Guid ProductId)
        {
            var productImages = await unitOfWork.Repository<ProductImage>()
                .GetAllAsync(x => x.ProductId == ProductId);

            if (productImages == null || !productImages.Any())
                return false;

            await unitOfWork.Repository<ProductImage>().RemoveRangeAsync(productImages);

            return true;

        }

        public async Task SaveImagesAsync(ProductImageAddRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var productImages = new List<ProductImage>();

            foreach (var image in request.Images)
            {
                var fileName = await fileServices.CreateFile(image);

                var productImage = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = fileName,
                    ProductId = request.ProductId
                };

                productImages.Add(productImage);
            }

            await unitOfWork.Repository<ProductImage>().AddRangeAsync(productImages);
        }
    }
}
