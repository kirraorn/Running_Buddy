using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class RoutePage : ContentPage
{
	public RoutePage()
	{
		InitializeComponent();
        BindingContext = new RoutePageViewModel();
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (BindingContext as RoutePageViewModel)?.RefreshPage();
    }

    private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {

    }

    private async void AddorEditRoutesClicked(object sender, EventArgs e)
    {
    // This pushes the RouteDetail page onto the stack
    await Shell.Current.GoToAsync("RouteDetail"); 
    }

    private async void EditClicked(object sender, EventArgs e)
    {
        int selectedId;
        if (sender is Button button)
        {
            object parameterValue = button.CommandParameter;
            selectedId = (int)parameterValue;
        }
        else
        {
            selectedId = 0;
        }
        await Shell.Current.GoToAsync($"RouteDetail?routeId={selectedId}");
    }


}
