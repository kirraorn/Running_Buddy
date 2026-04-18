using Microsoft.Extensions.Logging;
using RunningBuddy.ViewModels;
using RunningBuddy.Views;        

namespace RunningBuddy;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwsome7.ttf", "FontAwsome7");
            });

       
        builder.Services.AddTransient<ShoeDetailView>();
        builder.Services.AddTransient<ShoeDetailViewModel>();

     
        builder.Services.AddSingleton<ShoeClosetPage>();
        builder.Services.AddSingleton<ShoeClosetViewModel>();

        builder.Services.AddSingleton<CalendarPage>();
        builder.Services.AddSingleton<CalendarViewModel>();

        builder.Services.AddTransient<CalendarDetailPage>();
        builder.Services.AddTransient<CalendarDetailViewModel>();

        builder.Services.AddTransient<TrainingPlanPage>();
        builder.Services.AddTransient<TrainingPlanViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
