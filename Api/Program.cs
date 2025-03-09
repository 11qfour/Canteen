using ApiDomain;
using ApiDomain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiListContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information));


// Регистрируем репозитории
builder.Services.AddScoped<CartDetailsRepository>();
builder.Services.AddScoped<CartsRepository>();
builder.Services.AddScoped<OrdersRepository>();
builder.Services.AddScoped<OrdersDetailsRepository>();
builder.Services.AddScoped<CustomersRepository>();
builder.Services.AddScoped<CategoriesRepository>();
builder.Services.AddScoped<DishesRepository>();
builder.Services.AddScoped<EmployeesRepository>();

var app = builder.Build();



// Подключаем Swagger ВСЕГДА (не только в Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = "swagger"; // Доступно по /swagger/
});

// Обработчик ошибок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
