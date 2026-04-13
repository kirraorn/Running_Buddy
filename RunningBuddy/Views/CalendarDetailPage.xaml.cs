using RunningBuddy.ViewModels;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.Views;

public partial class CalendarDetailPage : ContentPage
{
	private readonly List<Route> _routes = new();
    private const string NoRoutesLabel = "No routes available";

	public CalendarDetailPage()
	{
		InitializeComponent();
        BindingContext = new CalendarDetailViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRoutesIntoPicker();
    }

    private void LoadRoutesIntoPicker()
    {
        _routes.Clear();

        var routeList = RouteServiceProxy.Current.RouteList ?? new List<Route>();
        _routes.AddRange(routeList.Where(r => !string.IsNullOrWhiteSpace(r.Name)));

        if (BindingContext is not CalendarDetailViewModel vm)
        {
            return;
        }

        if (_routes.Count > 0)
        {
            vm.SelectedRoute = _routes[0];
            RouteSelectButton.Text = $"{_routes[0].Name} ({_routes[0].Length:F1} mi)";
        }
        else
        {
            vm.SelectedRoute = null;
            RouteSelectButton.Text = NoRoutesLabel;
        }
    }

    private async void ChooseRouteClicked(object sender, EventArgs e)
    {
        if (BindingContext is not CalendarDetailViewModel vm)
        {
            return;
        }

        if (_routes.Count == 0)
        {
            await DisplayAlert("No Routes", "Create a route first on the Routes page.", "OK");
            return;
        }

        var options = _routes
            .Select(r => $"{r.Name} ({r.Length:F1} mi)")
            .ToArray();

        var selected = await DisplayActionSheet("Select Route", "Cancel", null, options);

        if (string.IsNullOrWhiteSpace(selected) || selected == "Cancel")
        {
            return;
        }

        var selectedIndex = Array.IndexOf(options, selected);
        if (selectedIndex < 0)
        {
            return;
        }

        var route = _routes[selectedIndex];
        vm.SelectedRoute = route;
        RouteSelectButton.Text = selected;
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as CalendarDetailViewModel;
        if (viewModel?.SelectedRoute == null)
        {
            await DisplayAlert("Error", "Please select a route", "OK");
            return;
        }

        // Add the workout to the calendar
        viewModel.AddWorkout();

        // Navigate back
        await Shell.Current.GoToAsync("..");
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
