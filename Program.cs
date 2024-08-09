var builder = WebApplication.CreateBuilder(args);

// Register services for generating API documentation via Swagger/OpenAPI.
// This is particularly useful for development environments where you
// want to provide an interactive UI to explore your API's capabilities.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register logging services to allow for the application to log various levels
// of information throughout its execution. Essential for debugging and monitoring.
builder.Services.AddLogging();

// Register the default in-memory implementation of IMemoryCache.
// This is required for caching mechanisms in the application, such as those used
// in the RateLimitingMiddleware to store request count information.
builder.Services.AddMemoryCache();

// Register WeatherService with dependency injection to manage HTTP client
// instances efficiently. This encapsulates all HTTP interactions in a single service,
// making it easier to manage, test, and reuse throughout the application.
builder.Services.AddHttpClient<WeatherService>();

// Build the application host, all configurations and services registrations are
// prepared to be used within the application's lifetime.
var app = builder.Build();

// Configure the HTTP request pipeline only if the application is in the development
// stage. This includes setting up Swagger, which provides a UI for testing API endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware for rate limiting requests to protect the application from
// excessive use and ensure fair usage of resources among clients.
// The parameters define maximum requests per IP within a one-minute interval.
app.UseMiddleware<RateLimitingMiddleware>(100, TimeSpan.FromMinutes(1));

// Enforce HTTPS redirection which improves security by redirecting all HTTP requests
// to HTTPS, preventing man-in-the-middle attacks and ensuring data integrity.
app.UseHttpsRedirection();

// Map an HTTP GET endpoint to handle current weather requests.
// This endpoint uses the WeatherService to fetch current weather data based on latitude and longitude.
// The returned data includes information such as temperature and weather conditions.
app.MapGet("api/weather/current", async (double lat, double lon, WeatherService weatherService) =>
{
    // Call the WeatherService to obtain weather data for the specified coordinates.
    var weather = await weatherService.GetCurrentWeather(lat, lon);

    // Respond with the fetched weather data in JSON format.
    return Results.Ok(weather);
});

// Run the application. This starts listening for incoming HTTP requests
// and serves them as configured above.
app.Run();
