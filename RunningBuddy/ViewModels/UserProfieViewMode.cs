namespace RunningBuddy.ViewModels;

public class UserProfieViewMode : ContentView
{
	public UserProfieViewMode()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}