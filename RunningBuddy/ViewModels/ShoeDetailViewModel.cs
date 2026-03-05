using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RunningBuddy.Services;
using RunningBuddy.Models;


namespace RunningBuddy.ViewModels
{
    internal class ShoeDetailViewModel
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


        public void AddOrUpdateTrip()
        {
            ShoeServiceProxy.Current.AddOrUpdate(Model);
        }


    }
}