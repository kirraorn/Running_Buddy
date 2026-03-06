namespace RunningBuddy.Views;

public partial class ShoeDetailView : ContentPage
{
	public ShoeDetailView()
	{
		InitializeComponent();
	}

	private async void OkClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ShoeCloset");
    }

	private async void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ShoeCloset");
    }
}