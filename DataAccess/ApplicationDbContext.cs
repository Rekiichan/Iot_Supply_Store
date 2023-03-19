using IotSupplyStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotSupplyStore.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DetailProduct> DetailsProducts { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<EmployeeRequest> EmployeeRequests { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
            .HasMany(p => p.DetailProductId) // Product có nhiều DetailProduct
            .WithOne(d => d.Product) // Mỗi DetailProduct chỉ thuộc về một Product
            .HasForeignKey(d => d.ProductId);
        }
    }
}
