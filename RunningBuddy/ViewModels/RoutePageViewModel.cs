using RunningBuddy.Models;
using RunningBuddy.Services;
using RunningBuddy.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningBuddy.ViewModels
{
    internal class RoutePageViewModel : INotifyPropertyChanged
    {
        private RouteServiceProxy _routeSvc;
        //private double total;
        //private Route a;


        public RoutePageViewModel()
        {
            _routeSvc = RouteServiceProxy.Current;
            //total = _routeSvc.TotalMPG;
        }

        public RouteDetailViewModel? SelectedRoute { get; set; }
        public ObservableCollection<RouteDetailViewModel> Routes
        {
            get
            {
                 var nonFavorites = _routeSvc.RouteList
                    .Where(r => r.IsFavorite == false)
                    .Select(r => new RouteDetailViewModel(r));
                _routeSvc.DisplayRoutes();

                return new ObservableCollection<RouteDetailViewModel>(nonFavorites);
            }
        }

        public ObservableCollection<RouteDetailViewModel> FavoriteRoutes
        {
            get
            {
                var favorites = _routeSvc.RouteList
                    .Where(r => r.IsFavorite)
                    .Select(r => new RouteDetailViewModel(r));
                _routeSvc.DisplayRoutes();

                return new ObservableCollection<RouteDetailViewModel>(favorites);
            }
        }

        public int TotalRoutes => _routeSvc.RouteList.Count;

        public double TotalMiles => _routeSvc.RouteList.Sum(r => r.Length);

        public int TotalFavorites => _routeSvc.RouteList.Count(r => r.IsFavorite);





        public int SelectedRouteId => SelectedRoute?.Model?.Id ?? 0;




        public void DeleteRoute()
        {
            if (SelectedRoute?.Model == null)
            {
                return;
            }

            RouteServiceProxy.Current.DeleteRoute(SelectedRoute.Model.Id);
            NotifyPropertyChanged(nameof(Routes));

        }

        public ICommand DeleteRouteCommand => new Command<RouteDetailViewModel>((route) =>
        {
            if (route?.Model == null) return;

            _routeSvc.DeleteRoute(route.Model.Id);
            RefreshPage();
        });




        //Alex was here

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(Routes));
            NotifyPropertyChanged(nameof(FavoriteRoutes));
            NotifyPropertyChanged(nameof(TotalRoutes));
            NotifyPropertyChanged(nameof(TotalMiles));
            NotifyPropertyChanged(nameof(TotalFavorites));



        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
