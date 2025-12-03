using MISA.Core.Interfaces.Repositories;
using MISA.Core.Interfaces.Services;
using MISA.Core.Services;
using MISA.Crm.Development.Extensions;
using MISA.Crm.Development.Middleware;
using MISA.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add Services for Controllers
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Cấu hình DI
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
var app = builder.Build();

// Sử dụng Exception Middleware (đặt đầu tiên để bắt tất cả exception)
app.UseExceptionMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
