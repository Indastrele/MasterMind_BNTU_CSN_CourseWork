using System.IO;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CourseWorkOnCSharp.Views;

public partial class WaitWindow : Window
{
    private int _lobbyID;
    private MainWindow _mainWindow;
    private Stream _endPoint;
    
    public WaitWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public WaitWindow(MainWindow mainWindow, int id)
    {
        InitializeComponent();
        
        _mainWindow = mainWindow;
        _endPoint = _mainWindow.EndPoint;
        _lobbyID = id;
        
        WaitResponse();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void WaitResponse()
    {
        var buffer = new byte[1024];
        var messageLength = await _endPoint.ReadAsync(buffer);
        var message = Encoding.UTF8.GetString(buffer, 0, messageLength).Trim().Split(",");

        if (message[0] == "START")
        {
            var decryptingWindow = new DecryptingWindow(_mainWindow, _lobbyID);
            decryptingWindow.Show();
            Close();
        }
    }
}