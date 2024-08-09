var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

// Register WeatherService with HttpClient
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RateLimitingMiddleware>(100, TimeSpan.FromMinutes(1));
app.UseHttpsRedirection();

app.MapGet("api/weather/current", async (double lat, double lon, WeatherService weatherService) =>
{
    var weather = await weatherService.GetCurrentWeather(lat, lon);
    return Results.Ok(weather);
});

app.Run();

