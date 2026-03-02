namespace RunningBuddy.ViewModels;

public partial class ManualWeatherInputView : ContentView
{
	public ManualWeatherInputView()
	{
		InitializeComponent();
	}

	private void SubmitClicked(object sender, EventArgs e)
	{
        Shell.Current.GoToAsync("//MainPage");
    }

}