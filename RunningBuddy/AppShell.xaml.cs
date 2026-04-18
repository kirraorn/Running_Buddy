using RunningBuddy.Views;

namespace RunningBuddy;


public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RouteDetail), typeof(RouteDetail));
        Routing.RegisterRoute("CalendarDetail", typeof(CalendarDetailPage));
        Routing.RegisterRoute("TrainingPlanPage", typeof(TrainingPlanPage));
	}

    private async void ProfileClicked(object sender, EventArgs e) =>
       await Shell.Current.GoToAsync("//UserProfile");
    private  void HomeClicked(object sender, EventArgs e) => 
         Shell.Current.GoToAsync("//MainPage");
    private async void CalendarClicked(object sender, EventArgs e) =>
            Shell.Current.GoToAsync("//CalendarPage");
    private async void ShoesClicked(object sender, EventArgs e) =>
         await Shell.Current.GoToAsync("//ShoeCloset");
    private async void RoutesClicked(object sender, EventArgs e) =>
         await Shell.Current.GoToAsync("//RoutePage");

    private async void AddorEditRoutesClicked(object sender, EventArgs e) =>
         await Shell.Current.GoToAsync("RouteDetail");

     
}
