namespace E_Shop.Domain.Models
{
    public class Product
    {
        public Guid Id {  get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
        public virtual ICollection<ProductImage> Images { get; set; } = [];
        public virtual ICollection<Wishlist> Wishlists { get; set; } = [];
    }
}
