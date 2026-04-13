using RunningBuddy.ViewModels;
using RunningBuddy.Services;

namespace RunningBuddy.Views;

public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
        BindingContext = new CalendarViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as CalendarViewModel)?.LoadCalendar();
        GenerateCalendarGrid();
    }

    private void GenerateCalendarGrid()
    {
        var viewModel = BindingContext as CalendarViewModel;
        if (viewModel == null) return;

        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();

        // Add row definitions for the calendar weeks
        int daysInMonth = DateTime.DaysInMonth(viewModel.CurrentDate.Year, viewModel.CurrentDate.Month);
        int firstDayOfWeek = (int)new DateTime(viewModel.CurrentDate.Year, viewModel.CurrentDate.Month, 1).DayOfWeek;
        int totalCells = firstDayOfWeek + daysInMonth;
        int rows = (int)Math.Ceiling(totalCells / 7.0);

        for (int i = 0; i < rows; i++)
        {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = 60 });
        }

        // Fill in days
        int day = 1;
        for (int i = 0; i < totalCells; i++)
        {
            int row = i / 7;
            int col = i % 7;

            if (i < firstDayOfWeek)
            {
                // Empty cell for days before month starts
                continue;
            }

            var dayButton = new Button
            {
                Text = day.ToString(),
                BackgroundColor = new DateTime(viewModel.CurrentDate.Year, viewModel.CurrentDate.Month, day) == DateTime.Today ? 
                    Color.FromArgb("#A3E635") : Color.FromArgb("#1A1A1A"),
                TextColor = new DateTime(viewModel.CurrentDate.Year, viewModel.CurrentDate.Month, day) == DateTime.Today ? 
                    Colors.Black : Colors.White,
                CornerRadius = 8,
                Padding = 5,
                CommandParameter = new DateTime(viewModel.CurrentDate.Year, viewModel.CurrentDate.Month, day)
            };

            CalendarGrid.Add(dayButton, col, row);
            day++;
        }
    }

    private void PreviousMonthClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as CalendarViewModel;
        viewModel?.PreviousMonth();
        GenerateCalendarGrid();
    }

    private void NextMonthClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as CalendarViewModel;
        viewModel?.NextMonth();
        GenerateCalendarGrid();
    }

    private async void AddWorkoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("CalendarDetail");
    }

    private async void DeleteWorkoutClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int workoutId)
        {
            return;
        }

        bool confirm = await DisplayAlert("Delete Workout", "Remove this workout from upcoming workouts?", "Delete", "Cancel");
        if (!confirm)
        {
            return;
        }

        WorkoutServiceProxy.Current.DeleteWorkout(workoutId);
        (BindingContext as CalendarViewModel)?.LoadCalendar();
    }
}
