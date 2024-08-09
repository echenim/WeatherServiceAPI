using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(config["OpenWeatherSettings:BaseUrl"]);
        _apiKey = config["OpenWeatherSettings:ApiKey"];
    }

    public async Task<string> GetCurrentWeather(double lat, double lon)
    {
        var response = await _httpClient.GetAsync($"weather?lat={lat}&lon={lon}&appid={_apiKey}&units=metric");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        dynamic weather = JsonConvert.DeserializeObject(content);

        double tempCelsius = weather.main.temp;
        double tempFahrenheit = CelsiusToFahrenheit(tempCelsius);
        string temperatureDescription = CategorizeTemperature(tempFahrenheit);

        return $"Weather: {weather.weather[0].main}, Temperature: {tempFahrenheit}°F | {temperatureDescription}";
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
