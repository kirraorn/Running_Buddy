using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Text;
using System.Threading.Tasks;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;


internal class UserProfileViewModel : INotifyPropertyChanged
{
    private UserServiceProxy _userSvc;
    private RouteServiceProxy _routSvc; //not implemented yet
    private ShoeClosetViewModel _shoeSvc; //not implemented yet
    private PrServiceProxy _prSvc;
    private WorkoutServiceProxy _workoutSvc;


    public UserProfileViewModel()
    {
        _userSvc = UserServiceProxy.Current;
        _prSvc = PrServiceProxy.Current;
        _routSvc = RouteServiceProxy.Current;
        _workoutSvc = WorkoutServiceProxy.Current;

        EditPRCommand = new Command<PrDetailViewModel>(async (vm) => await ExecuteEditPR(vm));
        DeletePRCommand = new Command<PrDetailViewModel>(async (vm) => await ExecuteDeletePR(vm));

   
        RefreshPRs();
        RefreshActivePlan();
    }

    //USER DATA ACCESS---------------------------------------------------------
    public String UserName
    {
        get
        {
            return _userSvc.MainUser.Name;
            //return "test";
        }
        set { //allow editing username
        if (_userSvc.MainUser.Name != value)
        {
    
            _userSvc.MainUser.Name = value;
            _userSvc.SaveAll();

            NotifyPropertyChanged();
            
        }
        }
    }

    public double ColdPref
    {
        get
        {
            return _userSvc.MainUser.ColdPreference;
        }

        set
        {
            // Clamp to valid slider range (1-5)
            double clamped = Math.Clamp(value, 1.0, 5.0);
            if (_userSvc.MainUser.ColdPreference != clamped)
            {
                _userSvc.MainUser.ColdPreference = clamped;
                _userSvc.SaveAll();
                NotifyPropertyChanged();
            }
        }
    }

    public Double TotalMiles
    {
        get
        {
            return _userSvc.MainUser.TotalMilage;
        }
    }


    //PR DATA ACCESS-----------------------------------------------------------
    public PrDetailViewModel SelectedRoute { get; set; } //Will be used when there is a edit route screen
   private ObservableCollection<PrDetailViewModel> _prsCollection;
   public ObservableCollection<PrDetailViewModel> PRs
    {
        get => _prsCollection;
        set
        {
            _prsCollection = value;
            NotifyPropertyChanged();
        }
    }

    public ICommand EditPRCommand { get; }
    public ICommand DeletePRCommand { get; }

    


    public void RefreshPRs()
    {
        var prList = _prSvc.PRList.Select(t => new PrDetailViewModel(t));
        PRs = new ObservableCollection<PrDetailViewModel>(prList);
        NotifyPropertyChanged(nameof(WeeklyWorkouts));
    }

    //TRAINING PLAN DATA ACCESS-------------------------------------------------
    private string _activePlanName = "No active plan";
    private string _activePlanProgress = "Start a training plan from the + New Plan button above.";

    public string ActivePlanName
    {
        get => _activePlanName;
        set
        {
            if (_activePlanName != value)
            {
                _activePlanName = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string ActivePlanProgress
    {
        get => _activePlanProgress;
        set
        {
            if (_activePlanProgress != value)
            {
                _activePlanProgress = value;
                NotifyPropertyChanged();
            }
        }
    }

    public void RefreshActivePlan()
    {
        var activePlan = TrainingPlanService.Current.GetActivePlan();
        if (activePlan != null)
        {
            int currentWeek = TrainingPlanService.Current.GetCurrentWeek(activePlan);
            ActivePlanName = $"{activePlan.PlanType}";
            ActivePlanProgress = $"Week {currentWeek} of {activePlan.DurationWeeks} • Started {activePlan.StartDate:MMM d, yyyy}";
        }
        else
        {
            ActivePlanName = "No active plan";
            ActivePlanProgress = "Start a training plan from the + New Plan button above.";
        }
    }

    private async Task ExecuteDeletePR(PrDetailViewModel vm)
    {
        if (vm?.Model == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert("Delete Record", "Delete this PR?", "Yes", "No");
        if (confirm)
        {
            _prSvc.DeletePR(vm.Model.Id);
            RefreshPRs(); // Update UI
        }
    }

    private async Task ExecuteEditPR(PrDetailViewModel vm)
    {
        if (vm?.Model == null) return;

        // Example: Edit Distance
        string result = await Application.Current.MainPage.DisplayPromptAsync("Edit PR", "Enter new distance:", "Save", "Cancel", vm.Model.Distance.ToString(), keyboard: Keyboard.Numeric);
        
        if (double.TryParse(result, out double newDist))
        {
            vm.Model.Distance = newDist;
            _prSvc.AddOrUpdate(vm.Model);
            RefreshPRs();
        }
    }

    // Workout history filtered to runs completed in the last 7 days.
    public ObservableCollection<Workout> WeeklyWorkouts
{
    get
    {
        var startDate = DateTime.Today.AddDays(-7);
        var endDate = DateTime.Today.AddDays(1);

        var history = (_workoutSvc.WorkoutList ?? new List<Workout>())
            .Where(w => w.Date.Date >= startDate && w.Date.Date < endDate)
            .OrderByDescending(w => w.Date)
            .ToList();

        return new ObservableCollection<Workout>(history);
    }
}

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}
