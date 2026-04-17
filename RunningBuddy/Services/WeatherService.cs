using System.Text.Json;
using RunningBuddy.Models;

namespace RunningBuddy.Services
{
    internal static class WeatherService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiKey = "82e735610b1449d8bcc00306260303";
        private const string BaseUrl = "https://api.weatherapi.com/v1/current.json";

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

        private const string ForecastUrl = "https://api.weatherapi.com/v1/forecast.json";

        /// <summary>
        /// Fetches forecast weather data for a specific date and hour (up to 3 days out).
        /// Returns the hourly forecast closest to the requested time.
        /// </summary>
        public static async Task<(Weather? Weather, string? Location)> GetForecastWeatherAsync(string zipCode, DateTime scheduledTime)
        {
            try
            {
                // WeatherAPI free tier supports up to 3 days of forecast
                string dateStr = scheduledTime.ToString("yyyy-MM-dd");
                string url = $"{ForecastUrl}?key={ApiKey}&q={zipCode}&dt={dateStr}&aqi=no";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return (null, null);

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var location = root.GetProperty("location");
                var forecastDay = root.GetProperty("forecast")
                                     .GetProperty("forecastday")[0];
                var hours = forecastDay.GetProperty("hour");

                // Find the hourly forecast closest to the scheduled time
                int targetHour = scheduledTime.Hour;
                JsonElement? bestHour = null;
                int bestDiff = int.MaxValue;

                foreach (var hour in hours.EnumerateArray())
                {
                    string timeStr = hour.GetProperty("time").GetString() ?? "";
                    // Format: "2026-04-18 14:00"
                    if (DateTime.TryParse(timeStr, out DateTime hourTime))
                    {
                        int diff = Math.Abs(hourTime.Hour - targetHour);
                        if (diff < bestDiff)
                        {
                            bestDiff = diff;
                            bestHour = hour;
                        }
                    }
                }

                if (bestHour == null) return (null, null);

                var hourData = bestHour.Value;
                var hourCondition = hourData.GetProperty("condition");

                var weather = new Weather
                {
                    currentTemp = hourData.GetProperty("temp_f").GetDouble(),
                    Humidity = hourData.GetProperty("humidity").GetDouble(),
                    windSpeed = hourData.GetProperty("wind_mph").GetDouble(),
                    VisibilityMiles = hourData.GetProperty("vis_miles").GetDouble(),
                    ConditionText = hourCondition.GetProperty("text").GetString() ?? "Unknown",
                    ConditionIconUrl = "https:" + (hourCondition.GetProperty("icon").GetString() ?? ""),
                };

                string locName = $"{location.GetProperty("name").GetString()}, {location.GetProperty("region").GetString()}";

                return (weather, locName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Forecast error: {ex.Message}");
                return (null, null);
            }
        }
    }
}
