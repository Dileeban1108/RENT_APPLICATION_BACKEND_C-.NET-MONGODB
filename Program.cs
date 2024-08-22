using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentApplication.Configurations;
using RentApplication.Models;
using RentApplication.Services;
using System.Text;

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
builder.Services.AddScoped<UserService>();

// Register the UserService to the DI container
builder.Services.AddScoped<UserService>();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add controllers and services to the container
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

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

// Use authentication and authorization
app.UseAuthentication(); // Add this line to enable JWT authentication
app.UseAuthorization();

// Use routing and map the controllers
app.UseRouting();
app.UseCors("AllowAll");

// Map controller routes
app.MapControllers();

app.Run();
