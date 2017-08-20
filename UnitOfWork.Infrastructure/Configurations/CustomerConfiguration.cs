using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UnitOfWork.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer.Customer>
    {
        public void Configure(EntityTypeBuilder<Customer.Customer> builder)
        {
            builder.Property(c => c.CustomerName).HasMaxLength(20);

            builder.HasOne(c => c.ShoppingCart)
                .WithOne(s => s.Customer)
                .HasForeignKey<ShoppingCart.ShoppingCart>(s => s.CustomerId);


            builder.HasMany(c => c.ShippingAddresses);
        }
    }
}