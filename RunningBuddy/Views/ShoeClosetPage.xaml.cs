using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class ShoeClosetPage : ContentPage
{
	public ShoeClosetPage()
	{
		InitializeComponent();
        BindingContext = new ShoeClosetViewModel();
    }
}