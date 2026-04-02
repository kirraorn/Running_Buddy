using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;


internal class MainPageViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    private RouteServiceProxy _routSvc;

    // Weather backing fields
    private string _weatherTemp = "—°F";
    private string _weatherCondition = "Loading...";
    private string _weatherIconUrl = "";
    private string _weatherHumidity = "—%";
    private string _weatherWind = "— MPH";
    private string _weatherVisibility = "— mi";
    private string _locationName = "";

    public MainPageViewModel()
    {
        _userSvc = UserServiceProxy.Current;
        _routSvc = RouteServiceProxy.Current;

        _ = LoadWeatherAsync(); //speed up test -Alex
    }


    //USER DATA ACCESS---------------------------------------------------------
    public String UserName
    {
        get
        {
            return _userSvc.MainUser.Name;
            //return "test";
        }
    }

    //WEATHER DATA ACCESS------------------------------------------------------
    public string WeatherTemp
    {
        get => _weatherTemp;
        set { _weatherTemp = value; NotifyPropertyChanged(); }
    }

    public string WeatherCondition
    {
        get => _weatherCondition;
        set { _weatherCondition = value; NotifyPropertyChanged(); }
    }

    public string WeatherIconUrl
    {
        get => _weatherIconUrl;
        set { _weatherIconUrl = value; NotifyPropertyChanged(); }
    }

    public string WeatherHumidity
    {
        get => _weatherHumidity;
        set { _weatherHumidity = value; NotifyPropertyChanged(); }
    }

    public string WeatherWind
    {
        get => _weatherWind;
        set { _weatherWind = value; NotifyPropertyChanged(); }
    }

    public string WeatherVisibility
    {
        get => _weatherVisibility;
        set { _weatherVisibility = value; NotifyPropertyChanged(); }
    }

    public string LocationName
    {
        get => _locationName;
        set { _locationName = value; NotifyPropertyChanged(); }
    }
    //ROUTE DATA ACCESS--------------------------------------------------------
    public RouteDetailViewModel SelectedRoute { get; set; } //Will be used when there is a edit route screen
    public ObservableCollection<RouteDetailViewModel> Routes
    {
        get
        {
            var Routes = _routSvc.RouteList.Select(t => new RouteDetailViewModel(t));
            _routSvc.DisplayRoutes();

            return new ObservableCollection<RouteDetailViewModel>(Routes);
        }
    }


    //WEATHER METHODS----------------------------------------------------------
    /// Loads weather from the API using the users saved zip code.
    public async Task LoadWeatherAsync()
    {
        string zipCode = _userSvc.MainUser.ZipCode;

        if (string.IsNullOrWhiteSpace(zipCode))
        {
            WeatherCondition = "Set your zip code";
            return;
        }

        try
        {
            // Start both tasks at the same time
            var weatherTask = WeatherService.GetCurrentWeatherAsync(zipCode);
            var locationTask = WeatherService.GetLocationNameAsync(zipCode);

            // Wait for both to finish in parallel
            await Task.WhenAll(weatherTask, locationTask);

            var weather = await weatherTask;
            var locName = await locationTask;

            if (weather != null)
            {
                WeatherTemp = $"{weather.currentTemp:F0}°F";
                WeatherCondition = weather.ConditionText;
                WeatherIconUrl = weather.ConditionIconUrl;
                WeatherHumidity = $"{weather.Humidity:F0}%";
                WeatherWind = $"{weather.windSpeed:F1} MPH";
                WeatherVisibility = $"{weather.VisibilityMiles:F0} mi";
                LocationName = locName ?? "Unknown Location";
            }
            else
            {
                WeatherCondition = "Unable to load weather";
            }
        }
        catch (Exception)
        {
            WeatherCondition = "Offline / Error";
        }
    }

    /// Asks for a zip code, saves it and reloads weather.
    public async Task SetZipCodeAsync()
    {
        string currentZip = _userSvc.MainUser.ZipCode;
        string prompt = string.IsNullOrWhiteSpace(currentZip)
            ? "Enter your zip code to get weather data:"
            : $"Current zip code: {currentZip}. Enter a new zip code:";

        string result = await Application.Current.MainPage.DisplayPromptAsync(
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

    //GENERAL FUNCTIONS--------------------------------------------------------
    public void RefreshPage()
    {
        NotifyPropertyChanged(nameof(UserName));


    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       
       
    }



}

