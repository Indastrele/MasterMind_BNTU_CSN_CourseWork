using System.IO;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;

namespace CourseWorkOnCSharp.Views;

public partial class MainWindow : Window
{
    private WelcomeWindow _welcomeWindow;
    private Stream _endPoint;

    public Stream EndPoint
    {
        get => _endPoint;
    }
    
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public MainWindow(WelcomeWindow entryWindow, Stream connection)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        _welcomeWindow = entryWindow;
        _endPoint = connection;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Close_Handler(object sender, WindowClosingEventArgs e)
    {
        _welcomeWindow.Show();
        _endPoint.WriteAsync("END\n"u8.ToArray());
        _endPoint.FlushAsync();
        Hide();
    }

    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        var joinWindow = new JoinWindow(this);
        joinWindow.Show();
        
        Hide();
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        await _endPoint.WriteAsync("CREATE\n"u8.ToArray());
        await _endPoint.FlushAsync();

        var buffer = new byte[1024];
        var count = await _endPoint.ReadAsync(buffer);
        var response = Encoding.UTF8.GetString(buffer, 0, count).Trim().Split(",");

        var lobbyWindow = new LobbyWindow(int.Parse(response[1]), this);
        lobbyWindow.Show();
        
        Hide();
    }
}