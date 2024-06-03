using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CourseWorkOnCSharp.Models;

namespace CourseWorkOnCSharp.Views;

public partial class DecryptingWindow : Window
{
    private MainWindow _mainWindow;
    private int _lobbyID;
    private Socket _endPoint;
    private StackPanel _correspondencePanel;
    private StackPanel _decryptPanel;
    private List<List<Button>> _buttons = new List<List<Button>>();
    private ColorChoosingWindow _colorChoosingWindow;
    private int _guessingIndex = 0;

    public DecryptingWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public DecryptingWindow(MainWindow mainWindow, int id)
    {
        InitializeComponent();
        
        _mainWindow = mainWindow;
        _endPoint = _mainWindow.EndPoint;
        _lobbyID = id;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _decryptPanel = this.FindControl<StackPanel>("DecryptPanel")!;
        _correspondencePanel = this.FindControl<StackPanel>("CorrespondencePanel")!;
        
        for (int i = 0; i < 9; i++)
        {
            _buttons.Add(new List<Button>());
            var decryptWrapPanel = new WrapPanel();

            for (int j = 0; j < 4; j++)
            {
                _buttons[i].Add(new Button
                {
                    Height = 50,
                    Width = 50,
                    CornerRadius = CornerRadius.Parse("25"),
                    BorderBrush = Brushes.Black,
                    BorderThickness = Thickness.Parse("2"),
                    Background = Brushes.LightGray,
                    Margin = Thickness.Parse("7")
                });

                _buttons[i][j].Click += Button_Click;
                
                decryptWrapPanel.Children.Add(_buttons[i][j]);
            }
            
            _decryptPanel.Children.Add(decryptWrapPanel);
        }

        for (int i = 1; i < _buttons.Count; i++)
        {
            foreach (var button in _buttons[i])
            {
                button.IsEnabled = false;
            }
        }
    }

    private void DisplayCorrespondence(int correct, int wrongPosition)
    {
        var correspondenceWrapPanel = new WrapPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            Margin = Thickness.Parse("7")
        };
        
        correspondenceWrapPanel.Children.Add(new Border
        {
            Width = 20,
            Height = 20,
            CornerRadius = CornerRadius.Parse("10"),
            Background = Brushes.LightGreen,
            BorderBrush = Brushes.Black,
            BorderThickness = Thickness.Parse("1.5")
        });
        
        correspondenceWrapPanel.Children.Add(new TextBlock
        {
            Text = correct.ToString() + " | "
        });
        
        correspondenceWrapPanel.Children.Add(new Border
        {
            Width = 20,
            Height = 20,
            CornerRadius = CornerRadius.Parse("10"),
            Background = Brushes.Yellow,
            BorderBrush = Brushes.Black,
            BorderThickness = Thickness.Parse("1.5")
        });
        
        correspondenceWrapPanel.Children.Add(new TextBlock
        {
            Text = wrongPosition.ToString()
        });
        
        _correspondencePanel.Children.Add(correspondenceWrapPanel);
    }
    
    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        _colorChoosingWindow = new ColorChoosingWindow((sender as Button).Background as IImmutableSolidColorBrush);
        await _colorChoosingWindow.ShowDialog(this);

        (sender as Button).Background = _colorChoosingWindow.ButtonColor;
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        var hasNone = false;

        foreach (var button in _buttons[_guessingIndex])
        {
            if (Equals(button.Background, Brushes.LightGray))
            {
                hasNone = true;
                break;
            }
        }
        
        if (hasNone) return;

        var message = $"COMBINATION,{_lobbyID},{_guessingIndex},";
        var nums = ColorTranslator.TranslateToUInt(_buttons[_guessingIndex]);

        for (var i = 0; i < nums.Count; i++)
        {
            message += $"{nums[i]}";

            if (i != nums.Count - 1) message += ";";
            else message += "\n";
        }

        await _endPoint.SendAsync(Encoding.UTF8.GetBytes(message));

        var buffer = new byte[1024];
        await _endPoint.ReceiveAsync(buffer);
        var response = Encoding.UTF8.GetString(buffer).Trim().Split(",");

        if (response[0] != "CORRESPONDENCE") return;

        if (int.Parse(response[1]) == 4)
        {
            var request = $"WIN,{_lobbyID}";
            await _endPoint.SendAsync(Encoding.UTF8.GetBytes(request));
            
            var resultWindow = new ResultWindow("Вы выиграли", _mainWindow);
            resultWindow.Show();
            
            Close();
        }

        DisplayCorrespondence(int.Parse(response[1]), int.Parse(response[2]));
        
        foreach (var button in _buttons[_guessingIndex])
        {
            button.IsEnabled = false;
        }

        if (_guessingIndex == 8)
        {
            var request = $"GIVEUP,{_lobbyID}";
            await _endPoint.SendAsync(Encoding.UTF8.GetBytes(request));
            
            var resultWindow = new ResultWindow("Вы проиграли", _mainWindow);
            resultWindow.Show();
            
            Close();
        }
        _guessingIndex++;
        foreach (var button in _buttons[_guessingIndex])
        {
            button.IsEnabled = true;
        }
    }

    private async void GiveUpButton_Click(object sender, RoutedEventArgs e)
    {
        var request = $"GIVEUP,{_lobbyID}";
        await _endPoint.SendAsync(Encoding.UTF8.GetBytes(request));
        
        var resultWindow = new ResultWindow("Вы проиграли", _mainWindow);
        resultWindow.Show();

        Close();
    }
}