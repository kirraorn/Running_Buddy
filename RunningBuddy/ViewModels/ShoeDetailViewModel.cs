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
            Model = new Route();

            DeleteCommand = new Command(DoDelete);

        }

        public ShoeDetailViewModel(int id)
        {
            Model = RouteServiceProxy.Current.GetById(id) ?? new Route();

            DeleteCommand = new Command(DoDelete);


        }

        public ShoeDetailViewModel(Route? model)
        {
            Model = model ?? new Route();
            DeleteCommand = new Command(DoDelete);


        }

        public void DoDelete()
        {
            RouteServiceProxy.Current.DeleteRoute(Model.Id); //was ID, might cause problems
        }

        // alex test fn
        public int DoEdit()
        {
            return Model.Id;
        }

        public Route? Model { get; set; }
        public ICommand? DeleteCommand { get; set; }


        public void AddOrUpdateTrip()
        {
            RouteServiceProxy.Current.AddOrUpdate(Model);
        }


    }
}
