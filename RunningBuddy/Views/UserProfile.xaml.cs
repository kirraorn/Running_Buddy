namespace RunningBuddy.Views;

public partial class UserProfile : ContentPage
{
	public UserProfile()
	{
		//This screen should allow the user to view Name, weekly summary, PRs, Total lifetime milage
		//  and a slider should be visible to edit the ColdSensitivityScore
		InitializeComponent();
	}

    private void HomeClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

}