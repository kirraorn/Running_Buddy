using Microsoft.Maui.Networking;
using System.Net.NetworkInformation; // Added this for NetworkInterface

namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
    // Initialize user and weather
    Models.User user = new Models.User(); 
    // Made this nullable to fix the warning CS8618
    Models.Weather? currentWeather;

    public MainPage()
    {
        InitializeComponent();
        CheckConnectivity();
    }

    private void CheckConnectivity()
    {
        // Using MAUI's built-in Connectivity API
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            // If no internet, go to manual input
            Shell.Current.GoToAsync("//ManualWeatherInputView");
        }
    }  

    // This helper method check if WiFi is specifically being used
    private bool IsWifiConnected()
    {
        var profiles = Connectivity.Current.ConnectionProfiles;
        return profiles.Contains(ConnectionProfile.WiFi);
    }

    private bool IsNetworkConnected() 
    {
       return NetworkInterface.GetIsNetworkAvailable();
    }

    // Navigation methods
    private async void ProfileClicked(object sender, EventArgs e) => 
        await Shell.Current.GoToAsync("//UserProfile");
    private async void HomeClicked(object sender, EventArgs e) => 
        await Shell.Current.GoToAsync("//MainPage");
    private async void CalendarClicked(object sender, EventArgs e) => 
        await Shell.Current.GoToAsync("//CalendarPage");
    private async void ShoesClicked(object sender, EventArgs e) => 
        await Shell.Current.GoToAsync("//ShoesPage");
    private async void RoutesClicked(object sender, EventArgs e) => 
        await Shell.Current.GoToAsync("//RoutesPage");
}