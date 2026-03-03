using E_Shop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Shop.Infrastructure.Data.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.ToTable("Addresses");
        }
    }
}
