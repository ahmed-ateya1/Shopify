using E_Shop.Application.Dtos.WishlistDtos;
using Mapster;

namespace E_Shop.Application.Services
{
    public class WishlistService(IUnitOfWork unitOfWork, ILogger<WishlistService> logger)
        : IWishlistService
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
        public async Task<WishlistResponse> AddToWishlistAsync(WishlistAddRequest request)
        {
            if(request == null) 
                throw new ArgumentNullException(nameof(request));

            var product = await unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Id == request.ProductId);

            if(product == null)
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");


            var existingWishlist = await unitOfWork.Repository<Wishlist>()
                .GetByAsync(w => w.UserId == request.UserId && w.ProductId == request.ProductId);

            if(existingWishlist != null)
                throw new InvalidOperationException("Product already in wishlist");

            var wishlist = request.Adapt<Wishlist>();   

            await ExecuteWithTransactionAsync(async () =>
            {
                await unitOfWork.Repository<Wishlist>().CreateAsync(wishlist);
            });

            return wishlist.Adapt<WishlistResponse>() with
            {
                ProductName = product.Name,
                ProductPrice = product.Price,
            };

        }

        public async Task<PaginatedResponse<WishlistResponse>> GetWishlistAsync
            (Expression<Func<Wishlist, bool>>? predicate = null, PaginationDto? pagination = null)
        {
            pagination = pagination ?? new PaginationDto();


            var wishlists = await unitOfWork.Repository<Wishlist>()
                 .GetAllAsync(predicate,
                 includeProperties: "Product,User,Product.Images",
                 pageSize: pagination.PageSize,
                 pageIndex: pagination.PageIndex,
                 sortBy: pagination.SortBy,
                 sortDirection: pagination.SortDirection);

            if (wishlists == null || !wishlists.Any())
            {
                return new PaginatedResponse<WishlistResponse>();
            }


            return new PaginatedResponse<WishlistResponse>
            {
                Items = wishlists.Select(w => w.Adapt<WishlistResponse>() with
                {
                    ProductName = w.Product.Name,
                    ProductPrice = w.Product.Price,
                    ProductImageUrls = w.Product.Images.Select(i => i.ImageUrl)
                }),
                TotalCount = wishlists.Count(),
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize,
            };


        }

        public async Task<WishlistResponse> GetWishlistByAsync(Expression<Func<Wishlist, bool>> predicate)
        {
            var wishlist = await unitOfWork.Repository<Wishlist>()
                .GetByAsync(predicate, includeProperties: "Product,User,Product.Images");

            if (wishlist == null)
            {
                throw new KeyNotFoundException("Wishlist item not found.");
            }

            return wishlist.Adapt<WishlistResponse>() with
            {
                ProductName = wishlist.Product.Name,
                ProductPrice = wishlist.Product.Price,
                ProductImageUrls = wishlist.Product.Images.Select(i => i.ImageUrl)
            };
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid wishlistId)
        {
            var wishlist = await unitOfWork.Repository<Wishlist>().GetByAsync(w => w.Id == wishlistId);

            if (wishlist == null)
            {
                return false;
            }

            await ExecuteWithTransactionAsync(async () =>
            {
                await unitOfWork.Repository<Wishlist>().DeleteAsync(wishlist);
            });

            return true;
        }
    }
}
