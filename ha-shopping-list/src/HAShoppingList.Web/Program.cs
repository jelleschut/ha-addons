using HAShoppingList.Infrastructure.DependencyInjection;
using HAShoppingList.Infrastructure.Persistence;
using HAShoppingList.Web.Api;
using HAShoppingList.Web.Components;
using HAShoppingList.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<StoreService>();
builder.Services.AddScoped<RecipeCheckedStateService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.ShowCloseIcon = false;
    config.SnackbarConfiguration.VisibleStateDuration = 2000;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/share/DataProtection-Keys"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ShoppingListDbContext>();
        var dbPath = context.Database.GetDbConnection().DataSource;
        var dbDir = Path.GetDirectoryName(dbPath);

        if (!string.IsNullOrEmpty(dbDir))
            Directory.CreateDirectory(dbDir);

        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database migratie mislukt bij opstarten");
        throw;
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("X-Frame-Options");
        var csp = context.Response.Headers.ContentSecurityPolicy.ToString();
        if (!string.IsNullOrEmpty(csp))
            context.Response.Headers.ContentSecurityPolicy =
                csp.Replace("frame-ancestors 'self'", "frame-ancestors *");
        return Task.CompletedTask;
    });
    await next();
});

app.MapShoppingListApi();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();