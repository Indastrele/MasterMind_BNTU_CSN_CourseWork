using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;

namespace CourseWorkOnCSharp.Views;

public partial class MainWindow : Window
{
    private WelcomeWindow _welcomeWindow;
    private Socket _connectionToServer;
    
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
        _connectionToServer = connection;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Close_Handler(object sender, WindowClosingEventArgs e)
    {
        _welcomeWindow.Show();
        _connectionToServer.Send("END\n"u8.ToArray());
        _connectionToServer.Shutdown(SocketShutdown.Both);
        _connectionToServer.Close();
        Hide();
    }
}