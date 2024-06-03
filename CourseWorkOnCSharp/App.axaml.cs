using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;
using CourseWorkOnCSharp.Views;

namespace CourseWorkOnCSharp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new WelcomeWindow()
            {
                DataContext = new WelcomeWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}