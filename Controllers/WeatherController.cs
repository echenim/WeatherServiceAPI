using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController()
    {
        _weatherService = new WeatherService("your_api_key_here");
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentWeather([FromQuery] double lat, [FromQuery] double lon)
    {
        var weather = await _weatherService.GetCurrentWeather(lat, lon);
        return Ok(weather);
    }
}
