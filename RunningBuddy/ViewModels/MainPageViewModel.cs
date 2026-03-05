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


internal class MainPageViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    private RouteServiceProxy _routSvc;


    public MainPageViewModel()
    {
        _userSvc = UserServiceProxy.Current;
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
    }


    //ROUTE DATA ACCESS--------------------------------------------------------
    public RouteDetailViewModel SelectedRoute { get; set; } //Will be used when there is a edit route screen
    public ObservableCollection<RouteDetailViewModel> Routes
    {
        get
        {
            var Routes = _routSvc.RouteList.Select(t => new RouteDetailViewModel(t));
            _routSvc.DisplayRoutes();

            return new ObservableCollection<RouteDetailViewModel>(Routes);
        }
    }


    //GENERAL FUNCTIONS--------------------------------------------------------
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       
       
    }



}
