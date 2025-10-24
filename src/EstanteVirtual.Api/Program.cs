using EstanteVirtual.Data.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Service Configuration =====

// Add Controllers support (for MVC-style API)
builder.Services.AddControllers();

// Configure PostgreSQL database context (T016, T019)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Swagger/OpenAPI documentation (T020)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Estante Virtual API",
        Version = "v1",
        Description = "RESTful API for managing personal book collection with reviews",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Estante Virtual MVP",
            Url = new Uri("https://github.com/felipebianchini2006/spec-driven-teste")
        }
    });

    // Enable XML documentation if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS for Blazor frontend (T021)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:7242", // Blazor HTTPS
                "http://localhost:5248"   // Blazor HTTP
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===== Middleware Pipeline =====

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Estante Virtual API v1");
        options.RoutePrefix = string.Empty; // Swagger at root URL in development
    });
}

// Global exception handling middleware (T022)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(error.Error, "Unhandled exception occurred");

            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = 500,
                Message = "An internal server error occurred. Please try again later."
            });
        }
    });
});

app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

// Enable authorization (for future use)
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }

