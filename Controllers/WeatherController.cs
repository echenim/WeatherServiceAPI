using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(WeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("current")]
    public async Task<IActionResult> Get([FromQuery] double lat, [FromQuery] double lon)
    {
        try
        {
            var weather = await _weatherService.GetCurrentWeather(lat, lon);
            if (weather == null)
            {
                _logger.LogWarning("Weather data was not retrieved successfully.");
                return StatusCode(500, "Weather data could not be retrieved.");
            }
            return Ok(weather);
        }
        catch (WeatherServiceException ex)
        {
            _logger.LogError(ex, "Failed to retrieve weather data: {Message}", ex.Message);
            // Use the StatusCode from the exception if available, otherwise default to 500
            var statusCode = ex.StatusCode ?? HttpStatusCode.InternalServerError;
            return StatusCode((int)statusCode, $"An error occurred while retrieving weather data: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weather data.");
            return StatusCode(500, "An error occurred while retrieving weather data.");
        }
    }
}
