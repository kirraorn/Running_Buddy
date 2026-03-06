using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class RoutePage : ContentPage
{
	public RoutePage()
	{
		InitializeComponent();
        BindingContext = new RoutePageViewModel();
    }

    private async void AddorEditRoutesClicked(object sender, EventArgs e)
{
    // This pushes the RouteDetail page onto the stack
    await Shell.Current.GoToAsync("RouteDetail"); 
}
}
