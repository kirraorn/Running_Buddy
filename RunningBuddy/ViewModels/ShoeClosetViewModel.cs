using RunningBuddy.Models;
using RunningBuddy.Services;
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
    internal class ShoeClosetViewModel : INotifyPropertyChanged
    {

        private ShoeServiceProxy _shoeSvc;

        public ShoeClosetViewModel()
        {
            _shoeSvc = ShoeServiceProxy.Current;
        }
       

        //SHOE DATA ACCESS--------------------------------------------------------
        public ShoeDetailViewModel SelectedShoe { get; set; } //Will be used when there is a edit route screen
        public ObservableCollection<ShoeDetailViewModel> Shoes
        {
            get
            {
                var Shoes = _shoeSvc.ShoeList.Select(t => new ShoeDetailViewModel(t));
                _shoeSvc.DisplayShoes();

                return new ObservableCollection<ShoeDetailViewModel>(Shoes);
            }
        }

        //GENERAL FUNCTIONS--------------------------------------------------------

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(Shoes));


        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }


    }
}
