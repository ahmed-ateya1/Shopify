namespace E_Shop.Application.ServicesContract
{
    public interface ICategoryService
    {
        Task<CategoryResponse> AddCategoryAsync(CategoryAddRequest request);
        Task<CategoryResponse> UpdateCategoryAsync(CategoryUpdateRequest request);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<CategoryResponse> GetCategoryByAsync(Expression<Func<Category, bool>> predicate);

        Task<PaginatedResponse<CategoryResponse>> GetAllCategoriesByAsync(
            Expression<Func<Category,bool>>? predicate = null , PaginationDto? pagination = null);

    }
}
