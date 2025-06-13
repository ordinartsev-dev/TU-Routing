using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controller and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddHttpClient<GraphHopperService>();
builder.Services.AddHttpClient<TransitRouteService>();
builder.Services.AddHttpClient<FindTheNearestStationService>();
builder.Services.AddScoped<HybridRouteService>();
builder.Services.AddScoped<FindTheNearestStationService>();
builder.Services.AddScoped<FindScooterService>();
builder.Services.AddScoped<ScooterRouteService>();
builder.Services.AddScoped<WalkingRouteService>();
builder.Services.AddScoped<HybridRouteServiceSeveralPoints>();

// Use connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));

builder.Services.AddScoped<FetchAllPointers>();

var app = builder.Build();

// автоматично створить/оновити таблиці (у Java це працює автоматично)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); 
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();