using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class ShoeClosetPage : ContentPage
{
	public ShoeClosetPage()
	{
		InitializeComponent();
        BindingContext = new ShoeClosetViewModel();
    }

	 private void AddClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ShoeDetailView");
    }



}