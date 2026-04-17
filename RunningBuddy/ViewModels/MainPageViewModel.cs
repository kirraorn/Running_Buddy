using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;

internal class MainPageViewModel : INotifyPropertyChanged
{
    private readonly UserServiceProxy _userSvc;
    private readonly RouteServiceProxy _routSvc;

    private string _userName = string.Empty;

    private string _weatherTemp = "—°F";
    private string _weatherCondition = "Loading...";
    private string _weatherIconUrl = string.Empty;
    private string _weatherHumidity = "—%";
    private string _weatherWind = "— MPH";
    private string _weatherVisibility = "— mi";
    private string _locationName = string.Empty;

    private Clothing _recommened;
    private string _hat = string.Empty;
    private string _top = string.Empty;
    private string _bottom = string.Empty;

    private ObservableCollection<RouteDetailViewModel> _routes = new();
    private RouteDetailViewModel? _selectedRoute;

    private string _todayWorkoutName = "No workout scheduled";
    private string _todayWorkoutDistance = "— miles";
    private bool _hasWorkoutToday;

    // Scheduled run fields
    private DateTime _scheduledRunDate = DateTime.Today;
    private TimeSpan _scheduledRunTime = new TimeSpan(7, 0, 0); // default 7:00 AM
    private string _scheduledHat = string.Empty;
    private string _scheduledTop = string.Empty;
    private string _scheduledBottom = string.Empty;
    private string _scheduledWeatherTemp = "—°F";
    private string _scheduledWeatherCondition = "";
    private string _scheduledWeatherIconUrl = string.Empty;
    private bool _hasScheduledForecast;
    private bool _isLoadingForecast;

    public MainPageViewModel()
    {
        _userSvc = UserServiceProxy.Current;
        _routSvc = RouteServiceProxy.Current;
        _recommened = new Clothing();

        UserName = _userSvc.MainUser.Name ?? string.Empty;
        RefreshRoutes();

        _ = LoadWeatherAsync();
        _ = LoadTodayWorkoutAsync();
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName == value)
            {
                return;
            }

            _userName = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherTemp
    {
        get => _weatherTemp;
        set
        {
            if (_weatherTemp == value)
            {
                return;
            }

            _weatherTemp = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherCondition
    {
        get => _weatherCondition;
        set
        {
            if (_weatherCondition == value)
            {
                return;
            }

            _weatherCondition = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherIconUrl
    {
        get => _weatherIconUrl;
        set
        {
            if (_weatherIconUrl == value)
            {
                return;
            }

            _weatherIconUrl = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherHumidity
    {
        get => _weatherHumidity;
        set
        {
            if (_weatherHumidity == value)
            {
                return;
            }

            _weatherHumidity = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherWind
    {
        get => _weatherWind;
        set
        {
            if (_weatherWind == value)
            {
                return;
            }

            _weatherWind = value;
            NotifyPropertyChanged();
        }
    }

    public string WeatherVisibility
    {
        get => _weatherVisibility;
        set
        {
            if (_weatherVisibility == value)
            {
                return;
            }

            _weatherVisibility = value;
            NotifyPropertyChanged();
        }
    }

    public string LocationName
    {
        get => _locationName;
        set
        {
            if (_locationName == value)
            {
                return;
            }

            _locationName = value;
            NotifyPropertyChanged();
        }
    }

    public string TodayWorkoutName
    {
        get => _todayWorkoutName;
        set
        {
            if (_todayWorkoutName == value)
            {
                return;
            }

            _todayWorkoutName = value;
            NotifyPropertyChanged();
        }
    }

    public string TodayWorkoutDistance
    {
        get => _todayWorkoutDistance;
        set
        {
            if (_todayWorkoutDistance == value)
            {
                return;
            }

            _todayWorkoutDistance = value;
            NotifyPropertyChanged();
        }
    }

    public bool HasWorkoutToday
    {
        get => _hasWorkoutToday;
        set
        {
            if (_hasWorkoutToday == value)
            {
                return;
            }

            _hasWorkoutToday = value;
            NotifyPropertyChanged();
        }
    }

    public string Hat
    {
        get => _hat;
        set
        {
            if (_hat == value)
            {
                return;
            }

            _hat = value;
            NotifyPropertyChanged();
        }
    }

    public string Top
    {
        get => _top;
        set
        {
            if (_top == value)
            {
                return;
            }

            _top = value;
            NotifyPropertyChanged();
        }
    }

    public string Bottom
    {
        get => _bottom;
        set
        {
            if (_bottom == value)
            {
                return;
            }

            _bottom = value;
            NotifyPropertyChanged();
        }
    }

    public ObservableCollection<RouteDetailViewModel> Routes
    {
        get => _routes;
        set
        {
            if (_routes == value)
            {
                return;
            }

            _routes = value;
            NotifyPropertyChanged();
        }
    }

    public RouteDetailViewModel? SelectedRoute
    {
        get => _selectedRoute;
        set
        {
            if (_selectedRoute == value)
            {
                return;
            }

            _selectedRoute = value;
            NotifyPropertyChanged();
        }
    }

    public async Task LoadWeatherAsync()
    {
        string zipCode = _userSvc.MainUser.ZipCode ?? string.Empty;
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return;
        }

        var (weather, locName) = await WeatherService.GetFullWeatherDataAsync(zipCode);
        if (weather is null)
        {
            return;
        }

        _recommened.setClothing(weather, _userSvc.MainUser.ColdPreference);

        Hat = _recommened.hat_gloves ?? string.Empty;
        Top = _recommened.top ?? string.Empty;
        Bottom = _recommened.bottom ?? string.Empty;

        LocationName = locName ?? string.Empty;
        WeatherTemp = $"{weather.currentTemp:F0}°F";
        WeatherCondition = weather.ConditionText ?? string.Empty;
        WeatherIconUrl = weather.ConditionIconUrl ?? string.Empty;
        WeatherHumidity = $"{weather.Humidity:F0}%";
        WeatherWind = $"{weather.windSpeed:F1} MPH";
        WeatherVisibility = $"{weather.VisibilityMiles:F0} mi";
    }

    public Task LoadTodayWorkoutAsync()
    {
        try
        {
            var workouts = WorkoutServiceProxy.Current.GetUpcomingWorkouts();
            var todayWorkout = workouts.FirstOrDefault(w => w.Date.Date == DateTime.Today);

            if (todayWorkout is not null)
            {
                TodayWorkoutName = todayWorkout.RouteName;
                TodayWorkoutDistance = $"{todayWorkout.Distance:F1} miles";
                HasWorkoutToday = true;
            }
            else
            {
                TodayWorkoutName = "No workout scheduled";
                TodayWorkoutDistance = "— miles";
                HasWorkoutToday = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading today's workout: {ex.Message}");
            TodayWorkoutName = "No workout scheduled";
            TodayWorkoutDistance = "— miles";
            HasWorkoutToday = false;
        }

        return Task.CompletedTask;
    }

    public async Task SetZipCodeAsync()
    {
        string currentZip = _userSvc.MainUser.ZipCode ?? string.Empty;
        string prompt = string.IsNullOrWhiteSpace(currentZip)
            ? "Enter your zip code to get weather data:"
            : $"Current zip code: {currentZip}. Enter a new zip code:";

        var page = Application.Current?.MainPage;
        if (page is null)
        {
            return;
        }

        string? result = await page.DisplayPromptAsync(
            "Weather Location",
            prompt,
            accept: "Save",
            cancel: "Cancel",
            placeholder: "e.g. 12345",
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrWhiteSpace(result))
        {
            _userSvc.MainUser.ZipCode = result.Trim();
            _userSvc.AddOrUpdate(_userSvc.MainUser);
            await LoadWeatherAsync();
        }
    }

    public void RefreshPage()
    {
        UserName = _userSvc.MainUser.Name ?? string.Empty;
        RefreshRoutes();
        _ = LoadWeatherAsync();
        _ = LoadTodayWorkoutAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RefreshRoutes()
    {
        _routSvc.DisplayRoutes();
        Routes = new ObservableCollection<RouteDetailViewModel>(
            _routSvc.RouteList.Select(route => new RouteDetailViewModel(route)));
    }

    // ── Scheduled Run Properties ──────────────────────────────────────

    public DateTime ScheduledRunDate
    {
        get => _scheduledRunDate;
        set
        {
            if (_scheduledRunDate != value)
            {
                _scheduledRunDate = value;
                NotifyPropertyChanged();
            }
        }
    }

    public DateTime MinScheduledDate => DateTime.Today;
    public DateTime MaxScheduledDate => DateTime.Today.AddDays(2); // 3 days out max (today + 2)

    public TimeSpan ScheduledRunTime
    {
        get => _scheduledRunTime;
        set
        {
            if (_scheduledRunTime != value)
            {
                _scheduledRunTime = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string ScheduledHat
    {
        get => _scheduledHat;
        set { if (_scheduledHat != value) { _scheduledHat = value; NotifyPropertyChanged(); } }
    }

    public string ScheduledTop
    {
        get => _scheduledTop;
        set { if (_scheduledTop != value) { _scheduledTop = value; NotifyPropertyChanged(); } }
    }

    public string ScheduledBottom
    {
        get => _scheduledBottom;
        set { if (_scheduledBottom != value) { _scheduledBottom = value; NotifyPropertyChanged(); } }
    }

    public string ScheduledWeatherTemp
    {
        get => _scheduledWeatherTemp;
        set { if (_scheduledWeatherTemp != value) { _scheduledWeatherTemp = value; NotifyPropertyChanged(); } }
    }

    public string ScheduledWeatherCondition
    {
        get => _scheduledWeatherCondition;
        set { if (_scheduledWeatherCondition != value) { _scheduledWeatherCondition = value; NotifyPropertyChanged(); } }
    }

    public string ScheduledWeatherIconUrl
    {
        get => _scheduledWeatherIconUrl;
        set { if (_scheduledWeatherIconUrl != value) { _scheduledWeatherIconUrl = value; NotifyPropertyChanged(); } }
    }

    public bool HasScheduledForecast
    {
        get => _hasScheduledForecast;
        set { if (_hasScheduledForecast != value) { _hasScheduledForecast = value; NotifyPropertyChanged(); } }
    }

    public bool IsLoadingForecast
    {
        get => _isLoadingForecast;
        set { if (_isLoadingForecast != value) { _isLoadingForecast = value; NotifyPropertyChanged(); } }
    }

    /// <summary>
    /// Fetches forecast weather for the scheduled date/time and recommends clothing.
    /// </summary>
    public async Task LoadScheduledWeatherAsync()
    {
        string zipCode = _userSvc.MainUser.ZipCode ?? string.Empty;
        if (string.IsNullOrWhiteSpace(zipCode)) return;

        IsLoadingForecast = true;
        HasScheduledForecast = false;

        try
        {
            DateTime scheduledDateTime = _scheduledRunDate.Date + _scheduledRunTime;

            var (weather, locName) = await WeatherService.GetForecastWeatherAsync(zipCode, scheduledDateTime);
            if (weather is null)
            {
                ScheduledWeatherTemp = "Forecast unavailable";
                ScheduledWeatherCondition = "Try a date within 3 days";
                return;
            }

            var scheduledClothing = new Clothing();
            scheduledClothing.setClothing(weather, _userSvc.MainUser.ColdPreference);

            ScheduledHat = scheduledClothing.hat_gloves;
            ScheduledTop = scheduledClothing.top;
            ScheduledBottom = scheduledClothing.bottom;
            ScheduledWeatherTemp = $"{weather.currentTemp:F0}°F";
            ScheduledWeatherCondition = weather.ConditionText;
            ScheduledWeatherIconUrl = weather.ConditionIconUrl;
            HasScheduledForecast = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Scheduled weather error: {ex.Message}");
            ScheduledWeatherTemp = "Error loading forecast";
        }
        finally
        {
            IsLoadingForecast = false;
        }
    }
}

