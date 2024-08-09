

public static class TemperatureManager
{
    public static double CelsiusToFahrenheit(double celsius)
    {
        return (celsius * 9 / 5) + 32;
    }

    public static string CategorizeTemperature(double tempFahrenheit)
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