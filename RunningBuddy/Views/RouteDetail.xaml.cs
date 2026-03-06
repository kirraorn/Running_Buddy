using RunningBuddy.ViewModels;
using RunningBuddy.Models;

namespace RunningBuddy.Views;

[QueryProperty(nameof(RouteId), "RouteId")]
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

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as RouteDetailViewModel;
        if (vm != null)
        {
            vm.AddOrUpdateTrip(); // Saves to your JSON via Proxy
            await Shell.Current.GoToAsync(".."); // Go back to dashboard
        }
    }
}
