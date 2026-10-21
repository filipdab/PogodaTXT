using System.Globalization;
using System.Net.Http.Json;

namespace PogodaTXT;

/// <summary>
/// Klient darmowego API Open-Meteo (https://open-meteo.com) – nie wymaga klucza API.
/// </summary>
public sealed class WeatherClient : IDisposable
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };

    public async Task<List<Location>> SearchCityAsync(string name, int count = 5)
    {
        // Pobieramy więcej wyników, bo część z nich odpadnie przy filtrowaniu
        var url = "https://geocoding-api.open-meteo.com/v1/search" +
                  $"?name={Uri.EscapeDataString(name)}&count={count * 3}&language=pl&format=json";
        var response = await _http.GetFromJsonAsync<GeocodingResponse>(url);
        var results = response?.Results ?? [];

        // Gdy nie ma żadnej miejscowości (np. szukano szczytu górskiego), zwracamy to, co jest
        var places = results.Where(l => l.IsPopulatedPlace).ToList();
        return (places.Count > 0 ? places : results).Take(count).ToList();
    }

    public async Task<ForecastResponse> GetForecastAsync(Location location, int days = 7)
    {
        var lat = location.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = location.Longitude.ToString(CultureInfo.InvariantCulture);
        var url = "https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,apparent_temperature,relative_humidity_2m,pressure_msl,wind_speed_10m,weather_code" +
                  "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max" +
                  "&hourly=weather_code,temperature_2m,precipitation,precipitation_probability,wind_speed_10m" +
                  $"&forecast_hours=24&timezone=auto&forecast_days={days}";
        var forecast = await _http.GetFromJsonAsync<ForecastResponse>(url);
        if (forecast?.Current is null || !IsComplete(forecast.Daily) || !IsComplete(forecast.Hourly))
            throw new InvalidDataException("Niekompletna odpowiedź z serwera pogody.");
        return forecast;
    }

    private static bool IsComplete(DailyWeather? d) =>
        d?.Dates is { Count: > 0 } dates &&
        AllHaveCount(dates.Count, d.WeatherCodes, d.TempMax, d.TempMin, d.Precipitation, d.PrecipitationChance);

    private static bool IsComplete(HourlyWeather? h) =>
        h?.Times is { Count: > 0 } times &&
        AllHaveCount(times.Count, h.WeatherCodes, h.Temperature, h.Precipitation, h.PrecipitationChance, h.WindSpeed);

    private static bool AllHaveCount(int count, params System.Collections.ICollection?[] lists) =>
        lists.All(l => l?.Count == count);

    public void Dispose() => _http.Dispose();
}
