using E_Shop.Domain.Models.Identity;

namespace E_Shop.Domain.Models
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; } = false;
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = [];
    }
}
