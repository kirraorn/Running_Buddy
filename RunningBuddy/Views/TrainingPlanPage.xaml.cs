using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class TrainingPlanPage : ContentPage
{
    public TrainingPlanPage()
    {
        InitializeComponent();
        BindingContext = new TrainingPlanViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as TrainingPlanViewModel)?.Refresh();
    }

    private async void StartPlanClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as TrainingPlanViewModel;
        if (vm == null) return;

        string warning = vm.HasActivePlan
            ? $"Start a {vm.SelectedPlanType} plan beginning {vm.SelectedStartDate:MMMM d, yyyy}?\n\nYour current plan and its workouts will be removed."
            : $"Start a {vm.SelectedPlanType} plan beginning {vm.SelectedStartDate:MMMM d, yyyy}?\n\nThis will add workouts to your calendar.";

        bool confirm = await DisplayAlert(
            "Start Training Plan",
            warning,
            "Start",
            "Cancel");

        if (!confirm) return;

        vm.CreatePlan();

        await DisplayAlert("Plan Created!", 
            $"Your {vm.SelectedPlanType} training plan has been created.\nWorkouts have been added to your calendar.", 
            "View Calendar");

        await Shell.Current.GoToAsync("//CalendarPage");
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
