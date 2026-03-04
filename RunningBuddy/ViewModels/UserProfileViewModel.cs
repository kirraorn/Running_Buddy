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


public class UserProfileViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    private RouteServiceProxy _routSvc; //not implemented yet
    private ShoeDetailViewModel _shoeSvc; //not implemented yet


    public UserProfileViewModel()
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
