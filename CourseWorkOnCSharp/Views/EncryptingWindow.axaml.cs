using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CourseWorkOnCSharp.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace CourseWorkOnCSharp.Views;

public partial class EncryptingWindow : Window
{
    private MainWindow _mainWindow;
    private Stream _endPoint;
    private int _lobbyID;
    private List<Button> _buttons = new List<Button>();
    private WrapPanel _encryptPanel;
    
    public EncryptingWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public EncryptingWindow(MainWindow mainWindow, int id)
    {
        InitializeComponent();
        
        _mainWindow = mainWindow;
        _endPoint = _mainWindow.EndPoint;
        _lobbyID = id;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _encryptPanel = this.FindControl<WrapPanel>("EncryptPanel")!;

        for (var i = 0; i < 4; i++)
        {
            _buttons.Add(new Button
            {
                Height = 40, 
                Width = 40, 
                Background = Brushes.LightGray, 
                CornerRadius = CornerRadius.Parse("20"),
                BorderBrush = Brushes.Black,
                BorderThickness = Thickness.Parse("1.5"),
                Margin = Thickness.Parse("5")
            });

            _buttons[i].Click += Button_Click;
            _encryptPanel.Children.Add(_buttons[i]);
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var colorChoosingWindow = new ColorChoosingWindow((sender as Button).Background as IImmutableSolidColorBrush);
        await colorChoosingWindow.ShowDialog(this);

        (sender as Button).Background = colorChoosingWindow.ButtonColor;
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        var hasNone = false;

        foreach (var button in _buttons)
        {
            if (Equals(button.Background, Brushes.LightGray))
            {
                hasNone = true;
                break;
            }
        }

        if (hasNone)
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Некорректные данные", "Необходимо выбрать цвет для всех кнопок", ButtonEnum.Ok)
                .ShowAsync();
            return;
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            var amount = 0;

            foreach (var button in _buttons)
            {
                if (Equals(button.Background, _buttons[i].Background)) amount++;
            }

            if (amount > 1)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Повторяющиеся цвета", "Нельзя, чтобы были кнопки одинакового цвета", ButtonEnum.Ok)
                    .ShowAsync();
                return;
            }
        }

        var message = $"COMBINATION,{_lobbyID},";
        var numbers = ColorTranslator.TranslateToUInt(_buttons);
        for (int i = 0; i < numbers.Count; i++)
        {
            message += $"{numbers[i]}";
            if (i == numbers.Count - 1)
            {
                message += "\n";
                continue;
            }

            message += ";";
        }

        try
        {
            await _endPoint.WriteAsync(Encoding.UTF8.GetBytes(message));
            await _endPoint.FlushAsync();
        }
        catch
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Ошибка отправки", "Не удалось отправить данные", ButtonEnum.Ok)
                .ShowAsync();
            _mainWindow.Show();
            Close();
            return;
        }
        
        var seekerWindow = new SeekerWindow(ColorTranslator.TranslateToUInt(_buttons), _mainWindow, _lobbyID);
        seekerWindow.Show();
        Close();
    }
}