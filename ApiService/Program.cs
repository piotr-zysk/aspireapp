using ApiService.Data;
using ApiService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("appdb")
                           ?? throw new InvalidOperationException(
                               "Connection string 'appdb' was not found. Ensure AppHost references the appdb resource.");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

var assetsGroup = app.MapGroup("/assets").WithTags("Assets");

assetsGroup.MapGet("/", async (AppDbContext dbContext) =>
{
    var assets = await dbContext.Assets.AsNoTracking().ToListAsync();
    return Results.Ok(assets);
});

assetsGroup.MapGet("/{id:int}", async (int id, AppDbContext dbContext) =>
{
    var asset = await dbContext.Assets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    return asset is null ? Results.NotFound() : Results.Ok(asset);
});

assetsGroup.MapPost("/", async (AssetCreateRequest request, AppDbContext dbContext) =>
{
    var asset = new Asset
    {
        Name = request.Name,
        Description = request.Description,
        Quantity = request.Quantity
    };

    dbContext.Assets.Add(asset);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/assets/{asset.Id}", asset);
});

assetsGroup.MapPut("/{id:int}", async (int id, AssetUpdateRequest request, AppDbContext dbContext) =>
{
    var asset = await dbContext.Assets.FirstOrDefaultAsync(x => x.Id == id);
    if (asset is null)
    {
        return Results.NotFound();
    }

    asset.Name = request.Name;
    asset.Description = request.Description;
    asset.Quantity = request.Quantity;

    await dbContext.SaveChangesAsync();
    return Results.Ok(asset);
});

assetsGroup.MapDelete("/{id:int}", async (int id, AppDbContext dbContext) =>
{
    var asset = await dbContext.Assets.FirstOrDefaultAsync(x => x.Id == id);
    if (asset is null)
    {
        return Results.NotFound();
    }

    dbContext.Assets.Remove(asset);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapHealthChecks("/health");

app.Run();

public record AssetCreateRequest(string Name, string Description, decimal Quantity);
public record AssetUpdateRequest(string Name, string Description, decimal Quantity);
