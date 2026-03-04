using Microsoft.AspNetCore.Http;

namespace E_Shop.Application.Dtos
{
    public record CategoryUpdateRequest(Guid Id,
        string Name,
        IFormFile? Image);

}
