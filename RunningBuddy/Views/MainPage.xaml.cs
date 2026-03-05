//using Microsoft.Maui.Networking;
using System.Net.NetworkInformation; // Added this for NetworkInterface
using RunningBuddy.ViewModels;
namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
	//Models.User user = new Models.User(); //currently creating an instance of the user class, to check connectivity, this should probably be refactored
	Models.Weather currentWeather;
	int count = 0;


    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
        CheckConnectivity();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        (BindingContext as MainPageViewModel)?.RefreshPage();
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
    /*
    private bool IsNetworkConnected() 
    {
       return NetworkInterface.GetIsNetworkAvailable();
    }
    */
    // Navigation methods
   
}