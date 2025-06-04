using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controller and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register GraphHopperService, TransitRouteService and FindTheNearestStationService
builder.Services.AddHttpClient<GraphHopperService>();
builder.Services.AddHttpClient<TransitRouteService>();
builder.Services.AddHttpClient<FindTheNearestStationService>();
builder.Services.AddScoped<HybridRouteService>();
builder.Services.AddScoped<FindTheNearestStationService>();
builder.Services.AddScoped<FindScooterService>();
builder.Services.AddScoped<ScooterRouteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
