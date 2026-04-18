//using Microsoft.Maui.Networking;
using System.Net.NetworkInformation; // Added this for NetworkInterface
using RunningBuddy.ViewModels;
namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
	//Models.User user = new Models.User(); //currently creating an instance of the user class, to check connectivity, this should probably be refactored
	Models.Weather currentWeather;


    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
        //CheckConnectivity();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var vm = BindingContext as MainPageViewModel;
        if (vm == null) return;

        vm.RefreshPage();

        var user = RunningBuddy.Services.UserServiceProxy.Current.MainUser;

        if (string.IsNullOrWhiteSpace(user.ZipCode))
        {
            // We DO want to await this because it's a UI prompt the user must interact with
            await vm.SetZipCodeAsync();
        }
        else
        {
            // DO NOT use 'await' here. 
            // This fires the task in the background so the UI renders immediately.
            _ = vm.LoadWeatherAsync();
        }
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
   
    // Event handler for the Change Location button
    private async void OnChangeLocationClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as MainPageViewModel;
        if (vm != null)
        {
            await vm.SetZipCodeAsync();
        }
    }

    // Event handler for the Get Forecast button (scheduled run)
    private async void OnGetForecastClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as MainPageViewModel;
        if (vm != null)
        {
            await vm.LoadScheduledWeatherAsync();
        }
    }

    
}
