using Microsoft.AspNetCore.Identity;

namespace E_Shop.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public virtual ICollection<Address> Addresses { get; set; } = [];
        public virtual ICollection<Order> Orders { get; set; } = [];
        public virtual ICollection<Wishlist> Wishlists { get; set; } = [];
    }
}
