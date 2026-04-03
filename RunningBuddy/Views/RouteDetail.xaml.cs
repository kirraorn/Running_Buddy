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
        SetupTerrainPicker();
    }
    internal RouteDetail(RouteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        SetupTerrainPicker();
    }

    private void SetupTerrainPicker()
    {
        TerrainPicker.ItemsSource = new List<string>
        {
            "Track","Asphalt","Gravel","Trail","Dirt","Grass"
        };
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
