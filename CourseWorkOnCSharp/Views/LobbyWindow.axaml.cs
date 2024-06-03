using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;

namespace CourseWorkOnCSharp.Views;

public partial class LobbyWindow : Window
{
    private Socket _endPoint;
    private CancellationTokenSource cts = new CancellationTokenSource();
    private int _lobbyID;
    private LobbyWindowViewModel _viewModel = new LobbyWindowViewModel();
    private MainWindow _mainWindow;
    
    public LobbyWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public LobbyWindow(int id, MainWindow mainWindow)
    {
        InitializeComponent();

        _mainWindow = mainWindow;
        _endPoint = _mainWindow.EndPoint;
        _viewModel.ID = id;
        DataContext = _viewModel;
        WaitResponse();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        cts.Cancel();
        _mainWindow.Show();
        Close();
    }

    private async void WaitResponse()
    {
        try
        {
            var buffer = new byte[1024];
            var responseLength = await _endPoint.ReceiveAsync(buffer, SocketFlags.None, cts.Token);
            var message = Encoding.UTF8.GetString(buffer).Trim().Split(',');

            switch (int.Parse(message[1]))
            {
                case 0:
                    var encryptingWindow = new EncryptingWindow(_mainWindow, _lobbyID);
                    encryptingWindow.Show();
                    break;
                case 1:
                    var waitWindow = new WaitWindow(_mainWindow, _lobbyID);
                    waitWindow.Show();
                    break;
            }

            Close();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Receive operation was canceled.");
        }
    }
}