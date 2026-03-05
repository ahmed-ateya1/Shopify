using E_Shop.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Shop.Domain.Models
{
    public class Wishlist
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
