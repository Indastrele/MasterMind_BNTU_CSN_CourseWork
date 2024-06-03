using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CourseWorkOnCSharp.Views;

public partial class JoinWindow : Window
{
    private MainWindow _mainWindow;
    private Socket _endPoint;
    private TextBox _id;
    
    public JoinWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public JoinWindow(MainWindow mainWindow)
    {
        InitializeComponent();

        _mainWindow = mainWindow;
        _endPoint = _mainWindow.EndPoint;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _id = this.FindControl<TextBox>("ID")!;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.Show();
        Close();
    }

    private async void Accept_Click(object sender, RoutedEventArgs e)
    {
        int lobbyID;
        if (!int.TryParse(_id.Text, out lobbyID))
        {
            await MessageBoxManager.GetMessageBoxStandard("Неправильный ввод",
                "Неправильно введён ID. Строка должна содеражть только целые числа", ButtonEnum.Ok).ShowAsync();
            return;
        }
        
        await _endPoint.SendAsync(Encoding.UTF8.GetBytes($"JOIN,{lobbyID}\n"));

        var buffer = new byte[1024];
        await _endPoint.ReceiveAsync(buffer);
        var message = Encoding.UTF8.GetString(buffer).Trim().Split(',');

        if (message[0] == "ERROR")
        {
            await MessageBoxManager.GetMessageBoxStandard("Неправильный ввод",
                message[1], ButtonEnum.Ok).ShowAsync();
            return;
        }
        
        if (message[0] != "START") return;
        switch (int.Parse(message[1]))
        {
            case 0:
                var encryptingWindow = new EncryptingWindow(_mainWindow, lobbyID);
                encryptingWindow.Show();
                break;
            case 1:
                var waitWindow = new WaitWindow(_mainWindow, lobbyID);
                waitWindow.Show();
                break;
        }
    }
}