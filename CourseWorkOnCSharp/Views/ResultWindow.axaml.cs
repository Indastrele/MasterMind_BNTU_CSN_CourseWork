using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;

namespace CourseWorkOnCSharp.Views;

public partial class ResultWindow : Window
{
    private MainWindow _mainWindow;
    private ResultWindowViewModel viewModel = new ResultWindowViewModel();
    
    public ResultWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public ResultWindow(string result, MainWindow mainWindow)
    {
        InitializeComponent();

        _mainWindow = mainWindow;
        viewModel = new ResultWindowViewModel(result);
        DataContext = viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ClosingWindow(object sender, WindowClosingEventArgs e)
    {
        _mainWindow.Show();
        Close();
    }
}