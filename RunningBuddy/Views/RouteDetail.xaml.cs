using RunningBuddy.ViewModels;
using RunningBuddy.Models;

namespace RunningBuddy.Views;

[QueryProperty(nameof(RouteId), "routeId")]
public partial class RouteDetail : ContentPage
{
    private int _routeId;
    public int RouteId
    {
        get => _routeId;
        set
        {
            _routeId = value;
            // Load the existing route if an ID was passed
            BindingContext = new RouteDetailViewModel(_routeId);
        }
    }

    public RouteDetail()
    {
        InitializeComponent();
        BindingContext = new RouteDetailViewModel();
        
        // Populate the picker with your Enum values from Route.cs
        TerrainPicker.ItemsSource = Enum.GetValues(typeof(Route.Terrain));
    }
    internal RouteDetail(RouteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Populate the picker with your Enum values from Route.cs
        TerrainPicker.ItemsSource = Enum.GetValues(typeof(Route.Terrain));
    }


    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is RouteDetailViewModel viewModel)
        {
            await viewModel.AddOrUpdateTrip();
            await Shell.Current.GoToAsync("..");
        }
    }


    private async void CancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
