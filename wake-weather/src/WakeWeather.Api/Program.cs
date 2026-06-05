using Microsoft.EntityFrameworkCore;
using WakeWeather.Api.Data;
using WakeWeather.Api.Endpoints;
using WakeWeather.Api.Services.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=wakeweather.db"));

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<OpenMeteoService>();
builder.Services.AddHttpClient<BuienradarService>();
builder.Services.AddScoped<WeatherAggregator>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:5261")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Identity is prepared here but not wired up yet — uncomment to activate auth:
// builder.Services.AddIdentityApiEndpoints<AppUser>()
//     .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await SeedData.SeedAsync(db);
}

app.UseCors();
app.UseHttpsRedirection();

// app.MapIdentityApi<AppUser>(); // uncomment when enabling auth

app.MapLocationEndpoints();
app.MapWeatherEndpoints();

app.Run();
