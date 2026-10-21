using System.Text.Json.Serialization;

namespace PogodaTXT;

public record GeocodingResponse(
    [property: JsonPropertyName("results")] List<Location>? Results);

public record Location(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude,
    [property: JsonPropertyName("country")] string? Country,
    [property: JsonPropertyName("admin1")] string? Region,
    [property: JsonPropertyName("feature_code")] string? FeatureCode)
{
    // Kody GeoNames: PPL* to miejscowości (miasta, wsie, dzielnice), reszta to np. parki, lotniska, góry
    public bool IsPopulatedPlace => FeatureCode?.StartsWith("PPL", StringComparison.Ordinal) == true;

    public string DisplayName =>
        string.Join(", ", new[] { Name, Region, Country }.Where(s => !string.IsNullOrWhiteSpace(s)));
}

public record ForecastResponse(
    [property: JsonPropertyName("current")] CurrentWeather Current,
    [property: JsonPropertyName("daily")] DailyWeather Daily);

public record CurrentWeather(
    [property: JsonPropertyName("time")] string Time,
    [property: JsonPropertyName("temperature_2m")] double Temperature,
    [property: JsonPropertyName("apparent_temperature")] double FeelsLike,
    [property: JsonPropertyName("relative_humidity_2m")] int Humidity,
    [property: JsonPropertyName("pressure_msl")] double Pressure,
    [property: JsonPropertyName("wind_speed_10m")] double WindSpeed,
    [property: JsonPropertyName("weather_code")] int WeatherCode);

public record DailyWeather(
    [property: JsonPropertyName("time")] List<string> Dates,
    [property: JsonPropertyName("weather_code")] List<int> WeatherCodes,
    [property: JsonPropertyName("temperature_2m_max")] List<double?> TempMax,
    [property: JsonPropertyName("temperature_2m_min")] List<double?> TempMin,
    [property: JsonPropertyName("precipitation_sum")] List<double?> Precipitation,
    [property: JsonPropertyName("precipitation_probability_max")] List<int?> PrecipitationChance);
