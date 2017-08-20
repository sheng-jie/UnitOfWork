using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UnitOfWork.Configurations
{
    public class GoodsConfiguration : IEntityTypeConfiguration<Goods.Goods>
    {
        public void Configure(EntityTypeBuilder<Goods.Goods> builder)
        {
            builder.Property(g => g.Name).HasMaxLength(200);

            builder.HasOne(g => g.GoodsCategory).WithMany(gc => gc.GoodsList);
        }
    }
}