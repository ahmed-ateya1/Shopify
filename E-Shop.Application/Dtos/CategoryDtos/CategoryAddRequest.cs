using Microsoft.AspNetCore.Http;

namespace E_Shop.Application.Dtos
{
    public record CategoryAddRequest(string Name,
        IFormFile? Image,
        Guid? ParentCategoryId);

}
