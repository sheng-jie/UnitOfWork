using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnitOfWork.ShoppingCart;

namespace UnitOfWork.Configurations
{
    public class ShoppingCartLineConfiguration : IEntityTypeConfiguration<ShoppingCartLine>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartLine> builder)
        {
            builder.HasOne(scl => scl.Goods);
        }
    }
}