using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentApplication.Configurations;
using RentApplication.Models;
using RentApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure the MySQL settings object using the "MySqlDatabase" section in appsettings.json
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MySqlDatabase"));

// Register the DbContext with the MySQL connection string
builder.Services.AddDbContext<RentApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlDatabase"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlDatabase"))
    ));

// Register the RentApplicationService to the DI container
builder.Services.AddScoped<RentApplicationService>();

// Add controllers and services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uncomment if using HTTPS redirection
// app.UseHttpsRedirection();

// Use routing and map the controllers
app.UseRouting();

app.UseAuthorization();

// Map controller routes
app.MapControllers();

app.Run();
