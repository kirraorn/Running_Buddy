using System.Collections.ObjectModel;
using RunningBuddy.Models;
using RunningBuddy.Services;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace RunningBuddy.ViewModels;

public class RoutePageViewModel : BindableObject
{
   private ObservableCollection<Route> _routes;
    public ObservableCollection<Route> Routes 
    { 
        get => _routes;
        set { _routes = value; OnPropertyChanged(); }
    }

    public int TotalRoutes => Routes?.Count ?? 0;
    public double TotalMiles => Routes?.Sum(r => r.Length) ?? 0;
    public int TotalFavorites => Routes?.Count(r => r.IsFavorite) ?? 0;

    // Use Shell to navigate to your detail/add page
    public ICommand CreateRouteCommand => new Command(async () => 
        await Shell.Current.GoToAsync($"{nameof(Views.RouteDetail)}?RouteId=0"));

    public RoutePageViewModel()
    {
        // Pull the actual list from your Proxy
        RefreshRoutes();
    }

    public void RefreshRoutes()
    {
        var list = RouteServiceProxy.Current.RouteList ?? new List<Route>();
        Routes = new ObservableCollection<Route>(list);
        
        // Notify the UI that stats have changed
        OnPropertyChanged(nameof(TotalRoutes));
        OnPropertyChanged(nameof(TotalMiles));
        OnPropertyChanged(nameof(TotalFavorites));
    }

    
}
