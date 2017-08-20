using Microsoft.EntityFrameworkCore;
using UnitOfWork.Configurations;
using UnitOfWork.Customer;
using UnitOfWork.Goods;
using UnitOfWork.ShoppingCart;

namespace UnitOfWork
{
    public class UnitOfWorkDbContext : DbContext
    {
        public UnitOfWorkDbContext(DbContextOptions<UnitOfWorkDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer.Customer> Customers { get; set; }
        public DbSet<Goods.Goods> Goods { get; set; }
        public DbSet<GoodsCategory> GoodsCategories { get; set; }
        public DbSet<ShoppingCart.ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartLine> ShoppingCartLines { get; set; }

        public DbSet<ContactAddress> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new GoodsConfiguration());
            builder.ApplyConfiguration(new GoodsCategoryConfiguration());
            builder.ApplyConfiguration(new ShoppingCartConfiguration());
            builder.ApplyConfiguration(new ShoppingCartLineConfiguration());
        }
    }
}