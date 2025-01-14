using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using  ThAmCo.Main.Models;
using ThAmCo.Main.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Data.SqlClient;
using ThAmCo.Main.Controllers;
using ThAmCo.Main.Services.UserService;
using ThAmCo.Main.Services.OrderService;
using ThAmCo.Main.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient("ThirdPartyAPI", client =>
{
    client.BaseAddress = new Uri("https://api.example.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

using (var connection = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    try
    {
        connection.Open();
        Console.WriteLine("Connection successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
    }
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<CoreServiceContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Local development environment using SQLite
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "coreservice.db");
        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
        // Production environment using Azure SQL
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,                 
                maxRetryDelay: TimeSpan.FromSeconds(2), 
                errorNumbersToAdd: null             
            );
        });
    }
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IUserService, FakeUserService>();
    builder.Services.AddScoped<IProductService, FakeProductService>();
    builder.Services.AddScoped<IOrderService, FakeOrderService>();
}
else
{
    builder.Services.AddHttpClient<IUserService, UserService>().AddPolicyHandler(GetRetryPolicy());
    builder.Services.AddHttpClient<IProductService, ProductService>().AddPolicyHandler(GetRetryPolicy());
    builder.Services.AddHttpClient<IOrderService, OrderService>().AddPolicyHandler(GetRetryPolicy());
}

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// Polly circuit breaker policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(5));
}