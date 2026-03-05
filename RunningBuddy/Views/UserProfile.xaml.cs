namespace RunningBuddy.Views;

using System.Threading.Tasks;
using RunningBuddy.Services;
using RunningBuddy.ViewModels;

public partial class UserProfile : ContentPage
{
	public UserProfile()
	{
		//This screen should allow the user to view Name, weekly summary, PRs, Total lifetime milage
		//  and a slider should be visible to edit the ColdSensitivityScore
		InitializeComponent();
		BindingContext = new UserProfileViewModel();
	}


	public async void EditClickedAsync(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Update Profile", "Enter your new username:", "Save", "Cancel", "New Username");
		if (!string.IsNullOrWhiteSpace(result))
		{
			// Cast the BindingContext to your ViewModel type
			if (BindingContext is UserProfileViewModel viewModel)
			{
				// Setting this triggers the 'set' block in the ViewModel
				viewModel.UserName = result; 
			}
		}
	}


}