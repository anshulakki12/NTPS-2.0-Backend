using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficerService.Data;
using OfficerService.Repositories;
using OfficerService.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers to handle proxy/load balancer scenarios
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    
    // Clear the known networks and proxies to accept headers from any source
    // In production, you should specify known proxies/networks for security
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    
    // Limit the number of forwarded headers to process (prevents header injection attacks)
    options.ForwardLimit = 2;
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Register repositories
builder.Services.AddScoped<ILogin, LoginRepository>();
builder.Services.AddScoped<IMasterRoleRepository, MasterRoleRepository>();
builder.Services.AddScoped<IMasterRolesLogsRepository, MasterRolesLogsRepository>();
builder.Services.AddScoped<IMasterDesignationRepository, MasterDesignationRepository>();
builder.Services.AddScoped<IMasterDesignationLogsRepository, MasterDesignationLogsRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IMasterModuleRepository, MasterModuleRepository>();
builder.Services.AddScoped<IMasterMenuRepository, MasterMenuRepository>();
builder.Services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<ISpeciesMappingRepository, SpeciesMappingRepository>();
builder.Services.AddScoped<IApplyTpNocRepository, ApplyTpNocRepository>();
builder.Services.AddScoped<IGovtDepotRepository, GovtDepotRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();


// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMasterRoleService, MasterRoleService>();
builder.Services.AddScoped<IMasterRolesLogsService, MasterRolesLogsService>();
builder.Services.AddScoped<IMasterDesignationService, MasterDesignationService>();
builder.Services.AddScoped<IMasterDesignationLogsService, MasterDesignationLogsService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IMasterModuleService, MasterModuleService>();
builder.Services.AddScoped<IMasterMenuService, MasterMenuService>();
builder.Services.AddScoped<IRoleMenuService, RoleMenuService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

// Read JWT configuration from appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validate JWT configuration
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing. Please check appsettings.json for Jwt:Key, Jwt:Issuer, and Jwt:Audience.");
}

// Add this to your Program.cs or Startup.cs
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});


// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production if using HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                           ?? new[] { "http://localhost:4200", "https://localhost:4200" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // Alternative policy for development (more permissive)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// IMPORTANT: Use forwarded headers middleware BEFORE any other middleware
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use more permissive CORS in development
    app.UseCors("AllowAll");
}
else
{
    // Use restricted CORS in production
    app.UseCors("AllowFrontend");
}

app.UseHttpsRedirection();

app.UseRouting();

// Enable session before authentication
app.UseSession();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
