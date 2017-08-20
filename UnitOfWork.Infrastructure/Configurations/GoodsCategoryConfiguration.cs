using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnitOfWork.Goods;

namespace UnitOfWork.Configurations
{
    public class GoodsCategoryConfiguration : IEntityTypeConfiguration<GoodsCategory>
    {
        public void Configure(EntityTypeBuilder<GoodsCategory> builder)
        {
            builder.Property(g => g.Name).HasMaxLength(200);
        }
    }
}