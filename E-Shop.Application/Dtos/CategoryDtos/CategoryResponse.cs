namespace E_Shop.Application.Dtos
{
    public record CategoryResponse(Guid Id,
        string Name,
        string? ImageUrl,
        Guid? ParentCategoryId,
        string? ParentCategoryName);

}
