using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels;

public class CalendarViewModel : INotifyPropertyChanged
{
    private DateTime currentDate = DateTime.Now;
    private ObservableCollection<RunEvent> upcomingRuns = new();

    public DateTime CurrentDate
    {
        get => currentDate;
        set
        {
            if (currentDate != value)
            {
                currentDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentMonthYear));
            }
        }
    }

    public string CurrentMonthYear
    {
        get => CurrentDate.ToString("MMMM yyyy");
    }

    public ObservableCollection<RunEvent> UpcomingRuns
    {
        get => upcomingRuns;
        set
        {
            if (upcomingRuns != value)
            {
                upcomingRuns = value;
                OnPropertyChanged();
            }
        }
    }

    public void LoadCalendar()
    {
        // Load upcoming runs from the workout service
        UpcomingRuns.Clear();

        try
        {
            var workouts = WorkoutServiceProxy.Current.GetUpcomingWorkouts();
            foreach (var workout in workouts)
            {
                UpcomingRuns.Add(new RunEvent
                {
                    Id = workout.Id,
                    Date = workout.Date,
                    Description = workout.RouteName,
                    Distance = workout.Distance
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading workouts: {ex.Message}");
        }
    }

    public void PreviousMonth()
    {
        CurrentDate = CurrentDate.AddMonths(-1);
    }

    public void NextMonth()
    {
        CurrentDate = CurrentDate.AddMonths(1);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RunEvent
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public double Distance { get; set; }
}
