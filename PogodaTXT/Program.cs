using System.Globalization;
using System.Text;
using PogodaTXT;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
// Polskie formatowanie liczb i dat niezależnie od ustawień systemu
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");

using var client = new WeatherClient();

// Miasto można podać jako argument: PogodaTXT Kraków
string? initialCity = args.Length > 0 ? string.Join(' ', args) : null;

WriteColored("=== PogodaTXT – prognoza pogody ===\n", ConsoleColor.Green);

while (true)
{
    string? query = initialCity;
    initialCity = null;

    if (query is null)
    {
        Console.Write("\nPodaj nazwę miasta (Enter = wyjście): ");
        query = Console.ReadLine()?.Trim();
    }

    if (string.IsNullOrEmpty(query))
        break;

    try
    {
        var locations = await client.SearchCityAsync(query);
        if (locations.Count == 0)
        {
            WriteColored($"Nie znaleziono miasta \"{query}\".\n", ConsoleColor.Red);
            continue;
        }

        var location = locations.Count == 1 ? locations[0] : ChooseLocation(locations);
        if (location is null)
            continue;

        var forecast = await client.GetForecastAsync(location);
        PrintForecast(location, forecast);
    }
    catch (HttpRequestException ex)
    {
        WriteColored($"Błąd połączenia: {ex.Message}\n", ConsoleColor.Red);
    }
    catch (TaskCanceledException)
    {
        WriteColored("Przekroczono czas oczekiwania na odpowiedź serwera.\n", ConsoleColor.Red);
    }
}

Console.WriteLine("Do zobaczenia!");

Location? ChooseLocation(List<Location> locations)
{
    Console.WriteLine("Znaleziono kilka miejscowości:");
    for (int i = 0; i < locations.Count; i++)
        Console.WriteLine($"  {i + 1}. {locations[i].DisplayName}");

    while (true)
    {
        Console.Write($"Wybierz numer (1-{locations.Count}, Enter = anuluj): ");
        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input))
            return null;
        if (int.TryParse(input, out var n) && n >= 1 && n <= locations.Count)
            return locations[n - 1];
        WriteColored("Nieprawidłowy wybór.\n", ConsoleColor.Red);
    }
}

void PrintForecast(Location location, ForecastResponse forecast)
{
    var c = forecast.Current;
    Console.WriteLine();
    WriteColored($"📍 {location.DisplayName}\n", ConsoleColor.Green);
    Console.WriteLine(new string('─', 60));
    Console.Write("Teraz: ");
    WriteColored($"{WeatherCodes.Describe(c.WeatherCode)}\n", WeatherCodes.Color(c.WeatherCode));
    Console.WriteLine($"  Temperatura:   {c.Temperature,6:0.0} °C (odczuwalna {c.FeelsLike:0.0} °C)");
    Console.WriteLine($"  Wilgotność:    {c.Humidity,6} %");
    Console.WriteLine($"  Ciśnienie:     {c.Pressure,6:0} hPa");
    Console.WriteLine($"  Wiatr:         {c.WindSpeed,6:0.0} km/h");
    Console.WriteLine(new string('─', 60));

    Console.WriteLine("Prognoza:");
    Console.WriteLine($"  {"Dzień",-16}{"Min",6}{"Max",6}{"Opady",9}{"Szansa",8}  Pogoda");
    var d = forecast.Daily;
    for (int i = 0; i < d.Dates.Count; i++)
    {
        var date = DateTime.ParseExact(d.Dates[i], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var day = i == 0 ? "Dziś" : i == 1 ? "Jutro" : date.ToString("ddd dd.MM");
        Console.Write($"  {day,-16}{Fmt(d.TempMin[i]),6}{Fmt(d.TempMax[i]),6}" +
                      $"{Fmt(d.Precipitation[i]) + " mm",9}{(d.PrecipitationChance[i]?.ToString() ?? "-") + "%",8}  ");
        WriteColored($"{WeatherCodes.Describe(d.WeatherCodes[i])}\n", WeatherCodes.Color(d.WeatherCodes[i]));
    }
}

static string Fmt(double? value) => value?.ToString("0.0") ?? "-";

static void WriteColored(string text, ConsoleColor color)
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = previous;
}
