using ApiDomain.Configurations;
using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartConfiguration());//добавление конфигураций
            modelBuilder.ApplyConfiguration(new CartDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
            base.OnModelCreating(modelBuilder);

            /*// Добавление начальных сотрудников
            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeID = 1, FullName = "Иван Иванов", Position = "Официант" },
                new Employee { EmployeeID = 2, FullName = "Петр Петров", Position = "Повар" }
            );*/
        }
    }
}


