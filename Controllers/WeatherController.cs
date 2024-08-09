using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Logging;

/// <summary>
/// Controller responsible for handling weather-related requests.
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherController"/> class.
    /// </summary>
    /// <param name="weatherService">The service to interact with weather data APIs.</param>
    /// <param name="logger">The logger for logging information and errors.</param>
    public WeatherController(WeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the current weather for the specified latitude and longitude.
    /// </summary>
    /// <param name="lat">The latitude of the location.</param>
    /// <param name="lon">The longitude of the location.</param>
    /// <returns>An IActionResult containing the weather data or an error message.</returns>
    [HttpGet("current")]
    public async Task<IActionResult> Get([FromQuery] double lat, [FromQuery] double lon)
    {
        try
        {
            // Attempt to retrieve weather data from the service
            var weather = await _weatherService.GetCurrentWeather(lat, lon);

            // If no data could be retrieved, log a warning and return a 500 error
            if (weather == null)
            {
                _logger.LogWarning("Weather data was not retrieved successfully.");
                return StatusCode(500, "Weather data could not be retrieved.");
            }

            // Successfully retrieved data, return OK status with weather data
            return Ok(weather);
        }
        catch (WeatherServiceException ex)
        {
            // Log specific weather service exceptions and return an appropriate HTTP status code
            _logger.LogError(ex, "Failed to retrieve weather data: {Message}", ex.Message);
            var statusCode = ex.StatusCode ?? HttpStatusCode.InternalServerError;
            return StatusCode((int)statusCode, $"An error occurred while retrieving weather data: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions and return a 500 Internal Server Error
            _logger.LogError(ex, "Failed to get weather data.");
            return StatusCode(500, "An error occurred while retrieving weather data.");
        }
    }
}
