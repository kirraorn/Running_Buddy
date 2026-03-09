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
        public static async Task<Weather?> GetCurrentWeatherAsync(string zipCode)
        {
            try
            {
                string url = $"{BaseUrl}?key={ApiKey}&q={zipCode}&aqi=no";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

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
                    ConditionIconUrl = condition.GetProperty("icon").GetString() ?? "",
                    harshConditions = condition.GetProperty("text").GetString() ?? ""
                };

                return weather;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// Returns the location name ("Tallahassee, Florida") from zip code
        public static async Task<string?> GetLocationNameAsync(string zipCode)
        {
            try
            {
                string url = $"{BaseUrl}?key={ApiKey}&q={zipCode}&aqi=no";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var location = doc.RootElement.GetProperty("location");

                string name = location.GetProperty("name").GetString() ?? "";
                string region = location.GetProperty("region").GetString() ?? "";

                return $"{name}, {region}";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
