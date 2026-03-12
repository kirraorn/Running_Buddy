using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;


internal class UserProfileViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    private RouteServiceProxy _routSvc; //not implemented yet
    private ShoeClosetViewModel _shoeSvc; //not implemented yet
    private PrServiceProxy _prSvc;


    public UserProfileViewModel()
    {
        _userSvc = UserServiceProxy.Current;
        _prSvc = PrServiceProxy.Current;
        _routSvc = RouteServiceProxy.Current;
    }

    //USER DATA ACCESS---------------------------------------------------------
    public String UserName
    {
        get
        {
            return _userSvc.MainUser.Name;
            //return "test";
        }
        set { //allow editing username
        if (_userSvc.MainUser.Name != value)
        {
    
            _userSvc.MainUser.Name = value;
            _userSvc.SaveAll();

            NotifyPropertyChanged();
            
        }
        }
    }

    //PR DATA ACCESS-----------------------------------------------------------
    public PrDetailViewModel SelectedRoute { get; set; } //Will be used when there is a edit route screen
    public ObservableCollection<PrDetailViewModel> PRs
    {
        get
        {

            var prs = _prSvc.PRList.Select(t => new PrDetailViewModel(t));
            _prSvc.DisplayPRs();

            return new ObservableCollection<PrDetailViewModel>(prs);
        }
    }

    // Workout history filtered to runs completed in the last 7 days.
    public ObservableCollection<RouteDetailViewModel> WeeklyWorkouts
    {
        get
        {
            var startDate = DateTime.Today.AddDays(-7);
            var today = DateTime.Now;

            var workouts = (_routSvc.RouteList ?? new List<Route>())
                .Where(route =>
                {
                    var runDate = route.Date != default ? route.Date : route.Time;
                    return runDate >= startDate && runDate <= today;
                })
                .OrderByDescending(route => route.Date != default ? route.Date : route.Time)
                .Select(route => new RouteDetailViewModel(route));

            return new ObservableCollection<RouteDetailViewModel>(workouts);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}
