using Microsoft.EntityFrameworkCore;
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

            builder.Entity<Customer.Customer>()
                .HasOne(c => c.ShoppingCart)
                .WithOne(s => s.Customer)
                .HasForeignKey<ShoppingCart.ShoppingCart>(s => s.CustomerId);

            builder.Entity<Customer.Customer>().HasMany(c => c.ShippingAddresses);
            builder.Entity<ShoppingCart.ShoppingCart>().HasMany(sc => sc.ShoppingCartLines).WithOne(cl => cl.ShoppingCart);
            builder.Entity<ShoppingCartLine>().HasOne(scl => scl.Goods);

            builder.Entity<Goods.Goods>().HasOne(g => g.GoodsCategory).WithMany(gc => gc.GoodsList);
        }
    }
}