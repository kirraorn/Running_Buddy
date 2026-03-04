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
    internal class PrDetailViewModel
    {
        public PrDetailViewModel()
        {
            Model = new PR();

            DeleteCommand = new Command(DoDelete);

        }

        public PrDetailViewModel(int id)
        {
            Model = PrServiceProxy.Current.GetById(id) ?? new PR();

            DeleteCommand = new Command(DoDelete);


        }

        public PrDetailViewModel(PR? model)
        {
            Model = model ?? new PR();
            DeleteCommand = new Command(DoDelete);


        }

        public void DoDelete()
        {
            PrServiceProxy.Current.DeletePR(Model.Id); //was ID, might cause problems
        }

        // alex test fn
        public int DoEdit()
        {
            return Model.Id;
        }

        public PR? Model { get; set; }
        public ICommand? DeleteCommand { get; set; }


        public void AddOrUpdateTrip()
        {
            PrServiceProxy.Current.AddOrUpdate(Model);
        }


    }
}
