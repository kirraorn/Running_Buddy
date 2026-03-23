using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RunningBuddy.Services;
using RunningBuddy.Models;


namespace RunningBuddy.ViewModels
{
    [QueryProperty(nameof(Model), "Shoe")]
    public class ShoeDetailViewModel : INotifyPropertyChanged
    {
        private Shoe? _model;


        public Shoe? Model 
        { 
            get => _model;
            set 
            {
                _model = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand? DeleteCommand { get; set; }
        public ICommand? EditCommand { get; set; }

        public ShoeDetailViewModel()
        {

            DeleteCommand = new Command(DoDelete);
        }


        public ShoeDetailViewModel(int id) : this()
        {
            Model = ShoeServiceProxy.Current.GetById(id) ?? new Shoe();
        }

        public void DoDelete()
        {
            if (Model != null)
            {
                ShoeServiceProxy.Current.DeleteShoe(Model.Id);
            }
        }

        public async Task AddOrUpdateShoe()
        {
            try 
            {
                if (Model != null)
                {
                    Model.setUsuage(); 
                    ShoeServiceProxy.Current.AddOrUpdate(Model);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error adding/updating shoe: {ex.Message}");
            }
        }

        public async Task AddOrUpdateTrip()
        {
            try {
                Model.setUsuage();
             ShoeServiceProxy.Current.AddOrUpdate(Model);
            }
    catch (Exception ex) {
        Console.WriteLine($"Error: {ex.Message}");
    }
        }

      
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}