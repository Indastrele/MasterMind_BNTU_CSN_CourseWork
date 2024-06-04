using System;
using System.Net;
using System.Net.Sockets;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CourseWorkOnCSharp.ViewModels;
using MsBox.Avalonia;

namespace CourseWorkOnCSharp.Views;

public partial class WelcomeWindow : Window
{
    private MainWindow _clientMenu;
    private TextBox _addressTextBox;
    
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _addressTextBox = this.FindControl<TextBox>("AddressTextBox")!;
    }
    
    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        var isConnected = IPAddress.TryParse(_addressTextBox.Text!, out var _);

        if (!isConnected)
        {
            MessageBoxManager.GetMessageBoxStandard(
                    "InputError",
                    $"Некорректно введён адрес {_addressTextBox.Text}")
                .ShowAsync();
            return;
        }
        
        var socket = new TcpClient(_addressTextBox.Text!, 11000);

        _clientMenu = new MainWindow(this, socket.GetStream())
        {
            DataContext = new MainWindowViewModel(),
        };
        
        _clientMenu.Show();
        Hide();
    }
}