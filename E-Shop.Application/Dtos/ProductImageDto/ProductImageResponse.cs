namespace E_Shop.Application.Dtos.ProductImageDto
{
    public record ProductImageResponse(Guid Id, IEnumerable<string> ImagesUrl);
}
