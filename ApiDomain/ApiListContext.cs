using ApiDomain.Configurations;
using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiDomain
{
    public class ApiListContext:DbContext
    {
        public ApiListContext(DbContextOptions<ApiListContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Dish> Dish { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<AuthUser> AuthUser { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartConfiguration());//добавление конфигураций
            modelBuilder.ApplyConfiguration(new CartDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new AuthUserConfiguration());
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var dateTimeProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in dateTimeProperties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}


