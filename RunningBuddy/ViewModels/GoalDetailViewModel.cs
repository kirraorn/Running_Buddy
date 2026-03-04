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
    internal class GoalDetailViewModel
    {
        public GoalDetailViewModel()
        {
            Model = new Goal();

            DeleteCommand = new Command(DoDelete);

        }

        public GoalDetailViewModel(int id)
        {
            Model = GoalServiceProxy.Current.GetById(id) ?? new Goal();

            DeleteCommand = new Command(DoDelete);


        }

        public GoalDetailViewModel(Goal? model)
        {
            Model = model ?? new Goal();
            DeleteCommand = new Command(DoDelete);


        }

        public void DoDelete()
        {
            GoalServiceProxy.Current.DeleteGoal(Model.Id); //was ID, might cause problems
        }

        // alex test fn
        public int DoEdit()
        {
            return Model.Id;
        }

        public Goal? Model { get; set; }
        public ICommand? DeleteCommand { get; set; }


        public void AddOrUpdateTrip()
        {
            GoalServiceProxy.Current.AddOrUpdate(Model);
        }


    }
}
