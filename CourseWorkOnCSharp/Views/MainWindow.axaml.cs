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
    private Socket _endPoint;

    public Socket EndPoint
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
    
    public MainWindow(WelcomeWindow entryWindow, Socket connection)
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
        _endPoint.Send("END\n"u8.ToArray());
        _endPoint.Shutdown(SocketShutdown.Both);
        _endPoint.Close();
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
        await _endPoint.SendAsync("CREATE\n"u8.ToArray());

        var buffer = new byte[1024];
        await _endPoint.ReceiveAsync(buffer);
        var response = Encoding.UTF8.GetString(buffer).Trim().Split(",");

        var lobbyWindow = new LobbyWindow(int.Parse(response[1]), this);
        lobbyWindow.Show();
        
        Hide();
    }
}