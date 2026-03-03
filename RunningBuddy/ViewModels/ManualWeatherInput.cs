namespace RunningBuddy.ViewModels;

public class ManualWeatherInput : ContentView
{
	public ManualWeatherInput()
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