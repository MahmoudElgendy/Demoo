using Demo.Api.Data;
using Demo.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.UseNetTopologySuite()
        ));
builder.Services.AddControllers().AddJsonOptions(o =>
{
    // keep camelCase for props (default in ASP.NET anyway)
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // ✅ read/write enums as strings
    // Option A: preserve enum case exactly ("Expert")
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // Option B (optional): if you want "expert" in JSON instead of "Expert"
    // o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

    // Optional: be forgiving with casing
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
}); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<AuthProfileMiddleware>();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Applies migrations if not yet applied
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


app.Run();
