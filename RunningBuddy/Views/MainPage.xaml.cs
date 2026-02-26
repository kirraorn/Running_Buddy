namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
	User user; //currently creating an instance of the user class, to check connectivity, this should probably be refactored
	int count = 0;
	
	public MainPage()
	{
		InitializeComponent();
	}

	private void ProfileClicked(object sender, EventArgs e)
	{
        Shell.Current.GoToAsync("//UserProfile");
    }

	private bool isNetworkConnected() //need to call every time page is refreshed
    {
        user.NetworkAccess = NetworkInterface.GetIsNetworkAvailable;
    }

}

