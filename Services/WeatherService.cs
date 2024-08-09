using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

/// <summary>
/// Provides weather-related services by interfacing with the OpenWeatherMap API.
/// </summary>
public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the WeatherService class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make requests to the OpenWeatherMap API.</param>
    /// <param name="config">Configuration that contains API key and base URL settings.</param>
    /// <param name="logger">Logger for capturing runtime information and errors.</param>
    public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config["OpenWeatherSettings:BaseUrl"]);
        _apiKey = config["OpenWeatherSettings:ApiKey"];
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the current weather information for a given latitude and longitude.
    /// </summary>
    /// <param name="lat">Latitude of the location.</param>
    /// <param name="lon">Longitude of the location.</param>
    /// <returns>A string describing the weather condition and temperature.</returns>
    public async Task<string> GetCurrentWeather(double lat, double lon)
    {
        try
        {
            var endpoint = $"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var parameters = new Dictionary<string, string> { { "lat", lat.ToString() }, { "lon", lon.ToString() } };

            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Weather API returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                throw new WeatherServiceException("Failed to retrieve weather data.", response.StatusCode, endpoint, parameters);
            }

            var content = await response.Content.ReadAsStringAsync();
            dynamic weather = JsonConvert.DeserializeObject(content);

            double tempCelsius = weather.main.temp;
            double tempFahrenheit = CelsiusToFahrenheit(tempCelsius);
            string temperatureDescription = CategorizeTemperature(tempFahrenheit);

            return $"Weather: {weather.weather[0].main}, Temperature: {tempFahrenheit}°F | {temperatureDescription}";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to Weather API failed.");
            throw new WeatherServiceException("HTTP request failed", HttpStatusCode.ServiceUnavailable, null, null, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching weather data.");
            throw new WeatherServiceException("HTTP request failed", HttpStatusCode.InternalServerError, null, null, ex);
        }
    }

    /// <summary>
    /// Converts Celsius to Fahrenheit.
    /// </summary>
    /// <param name="celsius">Temperature in Celsius.</param>
    /// <returns>Temperature in Fahrenheit.</returns>
    private static double CelsiusToFahrenheit(double celsius)
    {
        return (celsius * 9 / 5) + 32;
    }

    /// <summary>
    /// Categorizes temperature into human-readable formats.
    /// </summary>
    /// <param name="tempFahrenheit">Temperature in Fahrenheit.</param>
    /// <returns>A string that categorizes the temperature.</returns>
    private static string CategorizeTemperature(double tempFahrenheit)
    {
        if (tempFahrenheit <= 32) return "Freezing";
        if (tempFahrenheit <= 50) return "Cold";
        if (tempFahrenheit <= 60) return "Cool";
        if (tempFahrenheit <= 70) return "Mild";
        if (tempFahrenheit <= 80) return "Warm";
        if (tempFahrenheit <= 90) return "Hot";
        if (tempFahrenheit <= 100) return "Very Hot";
        return "Extreme Heat";
    }
}
