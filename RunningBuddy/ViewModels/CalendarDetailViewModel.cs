using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;

public class CalendarDetailViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Route> routes = new();
    private Route? selectedRoute;
    private DateTime selectedDate = DateTime.Today;
    private string notes = "";
    private int selectedRouteIndex = -1;


    public ObservableCollection<Route> Routes
    {
        get => routes;
        set
        {
            if (routes != value)
            {
                routes = value;
                OnPropertyChanged();
            }
        }
    }

    public Route? SelectedRoute
    {
        get => selectedRoute;
        set
        {
            if (selectedRoute != value)
            {
                selectedRoute = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRouteSelected));
            }
        }
    }
    public int SelectedRouteIndex
    {
        get => selectedRouteIndex;
        set
        {
            if (selectedRouteIndex != value)
            {
                selectedRouteIndex = value;
                OnPropertyChanged();
                
                // Update selected route when index changes
                if (value >= 0 && value < routes.Count)
                {
                    SelectedRoute = routes[value];
                }
            }
        }
    }


    public DateTime SelectedDate
    {
        get => selectedDate;
        set
        {
            if (selectedDate != value)
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }
    }

    public string Notes
    {
        get => notes;
        set
        {
            if (notes != value)
            {
                notes = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsRouteSelected
    {
        get => SelectedRoute != null;
    }

    public void LoadRoutes()
    {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var routeList = RouteServiceProxy.Current.RouteList;
                    Routes.Clear();
                    if (routeList != null && routeList.Any())
                    {
                        foreach (var route in routeList)
                        {
                            Routes.Add(route);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading routes: {ex.Message}");
                }
            });
    }

    public void AddWorkout()
    {
        if (SelectedRoute == null) return;

        var workout = new Workout
        {
            RouteId = SelectedRoute.Id,
            RouteName = SelectedRoute.Name,
            Distance = SelectedRoute.Length,
            Date = SelectedDate,
            Notes = Notes
        };

        WorkoutServiceProxy.Current.AddOrUpdate(workout);
        System.Diagnostics.Debug.WriteLine($"Added workout: {SelectedRoute.Name} on {SelectedDate}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
