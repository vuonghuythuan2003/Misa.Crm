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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Cấu hình JSON serialization dùng camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Cấu hình DI
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Cấu hình CORS để cho phép frontend từ localhost:5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();
// app.UseMiddleware<ExceptionMiddleware>();
// Sử dụng Exception Middleware (đặt đầu tiên để bắt tất cả exception)
app.UseExceptionMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
