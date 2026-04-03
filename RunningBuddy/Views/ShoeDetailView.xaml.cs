using RunningBuddy.ViewModels;
namespace RunningBuddy.Views;




public partial class ShoeDetailView : ContentPage
{
	public ShoeDetailView()
	{
		InitializeComponent();
		BindingContext = new ShoeDetailViewModel();
	}

   public ShoeDetailView(ShoeDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}


   private async void OkClicked(object sender, EventArgs e)
{
    if (BindingContext is  ShoeDetailViewModel viewModel)
    {
        await viewModel.AddOrUpdateTrip(); 
        await Shell.Current.GoToAsync("//ShoeCloset");
    }
}
    private async void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//ShoeCloset");
    }
}