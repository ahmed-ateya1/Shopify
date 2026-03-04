using E_Shop.Application.Exceptions;
using Mapster;

namespace E_Shop.Application.Services
{
    public class CategoryService(IUnitOfWork unitOfWork ,
        IFileServices fileServices,
        ILogger<CategoryService> logger)
        : ICategoryService
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
        public async Task<CategoryResponse> AddCategoryAsync(CategoryAddRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var category = request.Adapt<Category>();
            if (request.Image != null)
            {
                category.ImageUrl = await fileServices.CreateFile(request.Image);
            }

            await ExecuteWithTransactionAsync(async () =>
            {
                await unitOfWork.Repository<Category>().CreateAsync(category);
            });

            return category.Adapt<CategoryResponse>();

        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await unitOfWork.Repository<Category>()
                .GetByAsync(x => x.Id == id,includeProperties: "ParentCategory,SubCategories,Products");

            if(category == null)
            {
                throw new CategoryNotFound("Category not found.");
            }

            await ExecuteWithTransactionAsync(async () =>
            {
                if(category.Products != null && category.Products.Any())
                {
                    throw new InvalidOperationException("Cannot delete category with associated products.");
                }
                if(category.SubCategories != null && category.SubCategories.Any())
                {
                    throw new InvalidOperationException("Cannot delete category with associated subcategories.");
                }
                if(category.ImageUrl != null)
                {
                    await fileServices.DeleteFile(category.ImageUrl);
                }
                await unitOfWork.Repository<Category>().DeleteAsync(category);
            });

            return true;
        }

        public async Task<PaginatedResponse<CategoryResponse>> 
            GetAllCategoriesByAsync(Expression<Func<Category, bool>>? predicate = null,
            PaginationDto? pagination = null)
        {
            pagination = pagination ?? new PaginationDto();

            var categories = await unitOfWork.Repository<Category>()
                .GetAllAsync(predicate,
                includeProperties: "ParentCategory,SubCategories,Products",
                sortBy: pagination.SortBy,
                sortDirection: pagination.SortDirection,
                pageSize: pagination.PageSize,
                pageIndex: pagination.PageIndex);

            if(categories == null || !categories.Any())
            {
                return new PaginatedResponse<CategoryResponse>
                {
                    Items = new List<CategoryResponse>(),
                    TotalCount = 0
                };
            }

            return new PaginatedResponse<CategoryResponse>
            {
                Items = categories.Adapt<IEnumerable<CategoryResponse>>(),
                TotalCount = categories.Count(),
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize
            };
        }

        public async Task<CategoryResponse> GetCategoryByAsync(Expression<Func<Category, bool>> predicate)
        {
            var category = await unitOfWork.Repository<Category>()
                .GetByAsync(predicate, includeProperties: "ParentCategory,SubCategories,Products");

            if (category == null)
            {
                throw new CategoryNotFound("Category not found.");
            }

            return category.Adapt<CategoryResponse>();
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(CategoryUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var category = await unitOfWork.Repository<Category>()
                .GetByAsync(x => x.Id == request.Id, includeProperties: "ParentCategory,SubCategories,Products");

            if (category == null)
            {
                throw new CategoryNotFound("Category not found.");
            }

            category = request.Adapt(category);

            if (request.Image != null)
            {
                if (category.ImageUrl != null)
                {
                    await fileServices.DeleteFile(category.ImageUrl);
                }
                category.ImageUrl = await fileServices.CreateFile(request.Image);
            }

            await ExecuteWithTransactionAsync(async () =>
            {
                await unitOfWork.Repository<Category>().UpdateAsync(category);
            });

            return category.Adapt<CategoryResponse>();
        }
    }
}
