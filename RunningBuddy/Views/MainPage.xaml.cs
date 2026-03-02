using System.Net.NetworkInformation;

namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
	Models.User user = new Models.User(); //currently creating an instance of the user class, to check connectivity, this should probably be refactored
	Models.Weather currentWeather;
	int count = 0;
	
	
	public MainPage()
	{
		InitializeComponent();
		
        if (!isNetworkConnected()) //if offline go to manual weather input
			{
				Shell.Current.GoToAsync("//ManualWeatherInputView");
			}
		

	}

	private void ProfileClicked(object sender, EventArgs e)
	{
        Shell.Current.GoToAsync("//UserProfile");
    }

	private bool isNetworkConnected() //need to call every time page is refreshed
    {
       return user.NetworkAccess = NetworkInterface.GetIsNetworkAvailable();
    }

}

