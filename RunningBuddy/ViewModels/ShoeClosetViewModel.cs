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
        private int _totalShoes;
        private double _totalMilesLogged;
        private int _totalNeedReplacing;

        public ShoeClosetViewModel()
        {
            _shoeSvc = ShoeServiceProxy.Current;
            UpdateStats();
        }
       

        //SHOE DATA ACCESS--------------------------------------------------------
        public ShoeDetailViewModel? SelectedShoe { get; set; } //Will be used when there is a edit shoe screen
         public int SelecteShoeId => SelectedShoe?.Model?.Id ?? 0;
        
        public int TotalShoes
        {
            get => _totalShoes;
            set
            {
                if (_totalShoes != value)
                {
                    _totalShoes = value;
                    NotifyPropertyChanged(nameof(TotalShoes));
                }
            }
        }

        public double TotalMilesLogged
        {
            get => _totalMilesLogged;
            set
            {
                if (_totalMilesLogged != value)
                {
                    _totalMilesLogged = value;
                    NotifyPropertyChanged(nameof(TotalMilesLogged));
                }
            }
        }

        public int TotalNeedReplacing
        {
            get => _totalNeedReplacing;
            set
            {
                if (_totalNeedReplacing != value)
                {
                    _totalNeedReplacing = value;
                    NotifyPropertyChanged(nameof(TotalNeedReplacing));
                }
            }
        }

        public ObservableCollection<ShoeDetailViewModel> Shoes
        {
            get
            {
                var Shoes = _shoeSvc.ShoeList.Select(t => new ShoeDetailViewModel(t.Id));
                _shoeSvc.DisplayShoes();

                return new ObservableCollection<ShoeDetailViewModel>(Shoes);
            }
        }
        public ICommand DeleteShoeCommand => new Command<ShoeDetailViewModel>((shoe) =>
        {
            if (shoe == null) return;

            _shoeSvc.DeleteShoe(shoe.Model.Id);
            RefreshPage();
        });

  

        //GENERAL FUNCTIONS--------------------------------------------------------

        private void UpdateStats()
        {
            var shoes = _shoeSvc.ShoeList;
            TotalShoes = shoes.Count;
            TotalMilesLogged = shoes.Sum(s => s.CurrentMilage);
            TotalNeedReplacing = shoes.Count(s => s.isComplete());
        }

        public void RefreshPage()
        {
            UpdateStats();
            NotifyPropertyChanged(nameof(Shoes));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }


    }
}
