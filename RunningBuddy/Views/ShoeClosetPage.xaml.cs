using RunningBuddy.Models;
using RunningBuddy.ViewModels;

namespace RunningBuddy.Views;

public partial class ShoeClosetPage : ContentPage
{
	public ShoeClosetPage()
	{
		InitializeComponent();
        BindingContext = new ShoeClosetViewModel();
    }

	 private void AddClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ShoeDetailView");
    }

   private async void EditClicked(object sender, EventArgs e)
{
    var button = sender as Button;

    var selectedViewModel = button?.BindingContext as ShoeDetailViewModel; 

    if (selectedViewModel?.Model != null)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Shoe", selectedViewModel.Model }
        };


        await Shell.Current.GoToAsync("//ShoeDetailView", navigationParameter);
    }
}


}