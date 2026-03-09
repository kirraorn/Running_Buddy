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
    public class ShoeDetailViewModel
    {
        public ShoeDetailViewModel()
        {
            Model = new Shoe();

            DeleteCommand = new Command(DoDelete);

        }

        public ShoeDetailViewModel(int id)
        {
            Model = ShoeServiceProxy.Current.GetById(id) ?? new Shoe();

            DeleteCommand = new Command(DoDelete);


        }

        public ShoeDetailViewModel(Shoe? model)
        {
            Model = model ?? new Shoe();
            DeleteCommand = new Command(DoDelete);


        }

        public void DoDelete()
        {
            ShoeServiceProxy.Current.DeleteShoe(Model.Id); //was ID, might cause problems
        }

        // alex test fn
        public int DoEdit()
        {
            return Model.Id;
        }

        public Shoe? Model { get; set; }
        public ICommand? DeleteCommand { get; set; }


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

       public async Task AddOrUpdateShoe(){
        try{
                Model.setUsuage();
                ShoeServiceProxy.Current.AddOrUpdate(Model);
        }
        catch (Exception ex) {
      
        Console.WriteLine($"Error adding/updating shoe: {ex.Message}");
         }
        }

         public void RefreshPage()
        {
            NotifyPropertyChanged();


        }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       
       
    }



    }
}