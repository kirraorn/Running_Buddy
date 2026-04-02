using System.Text.Json;
using RunningBuddy.Models;

namespace RunningBuddy.Services
{
    internal static class WeatherService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiKey = "82e735610b1449d8bcc00306260303";
        private const string BaseUrl = "http://api.weatherapi.com/v1/current.json";

        /// Fetches current weather data from WeatherAPI for the given zip code.
        /// Only manual setup right now, maybe keep it that way? Unsure.
        public static async Task<(Weather? Weather, string? Location)> GetFullWeatherDataAsync(string zipCode)
        {
            try
            {
                string url = $"{BaseUrl}?key={ApiKey}&q={zipCode}&aqi=no";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return (null, null);

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var current = root.GetProperty("current");
                var condition = current.GetProperty("condition");
                var location = root.GetProperty("location");

                var weather = new Weather
                {
                    currentTemp = current.GetProperty("temp_f").GetDouble(),
                    Humidity = current.GetProperty("humidity").GetDouble(),
                    windSpeed = current.GetProperty("wind_mph").GetDouble(),
                    VisibilityMiles = current.GetProperty("vis_miles").GetDouble(),
                    ConditionText = condition.GetProperty("text").GetString() ?? "Unknown",
                    // FIX: Ensure the URL has https:
                    ConditionIconUrl = "https:" + (condition.GetProperty("icon").GetString() ?? ""),
                };

                string locName = $"{location.GetProperty("name").GetString()}, {location.GetProperty("region").GetString()}";

                return (weather, locName);
            }
            catch { return (null, null); }
        }
    }
}
