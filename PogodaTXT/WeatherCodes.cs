namespace PogodaTXT;

/// <summary>
/// Opisy kodów pogody WMO używanych przez Open-Meteo.
/// </summary>
public static class WeatherCodes
{
    public static string Describe(int code) => code switch
    {
        0 => "Bezchmurnie",
        1 => "Przeważnie pogodnie",
        2 => "Częściowe zachmurzenie",
        3 => "Pochmurno",
        45 or 48 => "Mgła",
        51 or 53 or 55 => "Mżawka",
        56 or 57 => "Marznąca mżawka",
        61 => "Słaby deszcz",
        63 => "Umiarkowany deszcz",
        65 => "Silny deszcz",
        66 or 67 => "Marznący deszcz",
        71 => "Słaby śnieg",
        73 => "Umiarkowany śnieg",
        75 => "Intensywny śnieg",
        77 => "Ziarnisty śnieg",
        80 or 81 or 82 => "Przelotny deszcz",
        85 or 86 => "Przelotny śnieg",
        95 => "Burza",
        96 or 99 => "Burza z gradem",
        _ => $"Nieznany ({code})"
    };

    public static ConsoleColor Color(int code) => code switch
    {
        0 or 1 => ConsoleColor.Yellow,
        2 or 3 or 45 or 48 => ConsoleColor.Gray,
        >= 71 and <= 77 or 85 or 86 => ConsoleColor.White,
        >= 95 => ConsoleColor.Magenta,
        _ => ConsoleColor.Cyan
    };
}
