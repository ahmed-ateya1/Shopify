using E_Shop.Domain.Enums;
using E_Shop.Domain.Models.Identity;

namespace E_Shop.Domain.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set;  }
        public decimal TotalAmount { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid ShippingAddressId { get; set; }
        public virtual Address ShippingAddress { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

    }
}
