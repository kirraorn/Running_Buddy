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


public class MainPageViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    //private RouteServiceProxy _routSvc; //does not exist yet


    public MainPageViewModel()
    {
        _userSvc = UserServiceProxy.Current;
    }

    public String UserName
    {
        get
        {
            return _userSvc.MainUser.Name;
            //return "test";
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}
