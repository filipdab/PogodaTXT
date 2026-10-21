# PogodaTXT

Konsolowa aplikacja do sprawdzania prognozy pogody dla wybranego miasta.

Dane pochodzą z darmowego API [Open-Meteo](https://open-meteo.com), które nie wymaga rejestracji ani klucza API.

## Funkcje

- wyszukiwanie miasta po nazwie (z polskimi nazwami miejscowości)
- wybór z listy, gdy nazwa pasuje do kilku miejsc (np. Warszawa w Polsce i na Ukrainie)
- aktualna pogoda: opis, temperatura i temperatura odczuwalna, wilgotność, ciśnienie, prędkość wiatru
- prognoza na 7 dni: temperatura min/max, suma opadów, szansa opadów, opis pogody
- kolorowanie opisów pogody w konsoli
- sprawdzanie kolejnych miast bez ponownego uruchamiania

## Wymagania

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- połączenie z internetem

## Uruchomienie

```bash
git clone https://github.com/filipdab/PogodaTXT.git
cd PogodaTXT
dotnet run --project PogodaTXT
```

Miasto można podać od razu jako argument:

```bash
dotnet run --project PogodaTXT -- Kraków
```

Można też otworzyć `PogodaTXT.slnx` w Visual Studio i uruchomić projekt klawiszem F5.

## Przykład

```
=== PogodaTXT – prognoza pogody ===

Podaj nazwę miasta (Enter = wyjście): Warszawa
Znaleziono kilka miejscowości:
  1. Warszawa, Województwo mazowieckie, Polska
  2. Warszawa, Obwód doniecki, Ukraina
Wybierz numer (1-2, Enter = anuluj): 1

📍 Warszawa, Województwo mazowieckie, Polska
────────────────────────────────────────────────────────────
Teraz: Pochmurno
  Temperatura:     12.4 °C (odczuwalna 9.8 °C)
  Wilgotność:        84 %
  Ciśnienie:       1020 hPa
  Wiatr:           17.3 km/h
────────────────────────────────────────────────────────────
Prognoza:
  Dzień              Min   Max    Opady  Szansa  Pogoda
  Dziś              11.4  16.6   0.0 mm      2%  Pochmurno
  Jutro              8.8  15.3  12.9 mm     97%  Umiarkowany deszcz
  ...
```

## Struktura projektu

| Plik | Opis |
|---|---|
| `PogodaTXT/Program.cs` | obsługa konsoli, wybór miasta, wyświetlanie prognozy |
| `PogodaTXT/WeatherClient.cs` | zapytania do API Open-Meteo (geokodowanie i prognoza) |
| `PogodaTXT/Models.cs` | modele danych odpowiedzi API |
| `PogodaTXT/WeatherCodes.cs` | polskie opisy i kolory kodów pogody WMO |

## Licencja

Projekt jest udostępniony na licencji MIT – szczegóły w pliku [LICENSE](LICENSE).
