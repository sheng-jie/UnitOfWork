using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UnitOfWork.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart.ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart.ShoppingCart> builder)
        {
            builder.HasMany(sc => sc.ShoppingCartLines).WithOne(cl => cl.ShoppingCart);
        }
    }
}