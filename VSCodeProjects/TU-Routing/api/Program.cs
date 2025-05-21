using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Регистрируем GraphHopperService
//builder.Services.AddScoped<GraphHopperService>();
builder.Services.AddHttpClient<GraphHopperService>();

// ✅ Регистрируем TransitRouteService
//builder.Services.AddScoped<TransitRouteService>();
builder.Services.AddHttpClient<TransitRouteService>();
builder.Services.AddHttpClient<FindTheNearestStationService>();
builder.Services.AddScoped<HybridRouteService>();
builder.Services.AddScoped<FindTheNearestStationService>();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Маршруты к контроллерам
app.MapControllers();

app.Run();
