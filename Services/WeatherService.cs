using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration config, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config["OpenWeatherSettings:BaseUrl"]);
        _apiKey = config["OpenWeatherSettings:ApiKey"];
        _logger = logger;
    }

    public async Task<string> GetCurrentWeather(double lat, double lon)
    {

        try
        {
            var endpoint = $"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric";
            var parameters = new Dictionary<string, string>{
                {"lat", lat.ToString()},
                {"lon", lon.ToString()}
            };

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


    private static double CelsiusToFahrenheit(double celsius)
    {
        return (celsius * 9 / 5) + 32;
    }

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
