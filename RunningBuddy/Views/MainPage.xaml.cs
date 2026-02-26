namespace RunningBuddy.Views;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void ProfileClicked(object sender, EventArgs e)
	{
        Shell.Current.GoToAsync("//UserProfile");
    }

}

