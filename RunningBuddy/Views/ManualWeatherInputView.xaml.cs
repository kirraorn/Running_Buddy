namespace RunningBuddy.Views;

public partial class ManualWeatherInputView : ContentPage
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