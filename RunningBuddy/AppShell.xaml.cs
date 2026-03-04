namespace RunningBuddy;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

    private async void ProfileClicked(object sender, EventArgs e) =>
       await Shell.Current.GoToAsync("//UserProfile");
    private  void HomeClicked(object sender, EventArgs e) => 
         Shell.Current.GoToAsync("//MainPage");
    private async void CalendarClicked(object sender, EventArgs e) =>
            Shell.Current.GoToAsync("//CalendarPage");
    private async void ShoesClicked(object sender, EventArgs e) =>
         await Shell.Current.GoToAsync("//ShoesPage");
    private async void RoutesClicked(object sender, EventArgs e) =>
         await Shell.Current.GoToAsync("//RoutesPage");
}
