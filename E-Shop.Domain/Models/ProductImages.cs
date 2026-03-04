namespace E_Shop.Domain.Models
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
