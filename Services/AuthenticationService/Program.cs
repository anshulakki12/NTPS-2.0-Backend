using AuthenticationService.Data;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // keep as required
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & app services
builder.Services.AddScoped<IMasterRegistrationRepository, MasterRegistrationRepository>();
builder.Services.AddScoped<IOtpService, OtpService>();
// Use the exact implementation name you have in your project
builder.Services.AddScoped<IPasswordService, PasswodService>(); // keep same as your project
builder.Services.AddScoped<ITokenService, TokenService>();

// JWT Authentication configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JWT Key is not configured. Add Jwt:Key to appsettings or environment.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

// Middleware order important:
app.UseCors("AllowAll");

app.UseRouting();

// enable authentication and session BEFORE controllers
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
