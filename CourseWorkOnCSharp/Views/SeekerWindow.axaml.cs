using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CourseWorkOnCSharp.Models;

namespace CourseWorkOnCSharp.Views;

public partial class SeekerWindow : Window
{
    private MainWindow _mainWindow;
    private Socket _endPoint;
    private int _lobbyID;
    private List<Border> _cypher = new List<Border>();
    private List<List<Border>> _decrypt = new List<List<Border>>();
    private WrapPanel _cypherPanel;
    private StackPanel _decryptPanel;
    
    public SeekerWindow()
    {
        InitializeComponent();

        for (int i = 0; i < 4; i++)
        {
            _cypher.Add(new Border()
            {
                Height = 50,
                Width = 50,
                CornerRadius = CornerRadius.Parse("25"),
                Background = Brushes.LightGray,
                BorderThickness = Thickness.Parse("2"),
                BorderBrush = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            _cypherPanel.Children.Add(_cypher[i]);
        }
        
        
    }

    public SeekerWindow(List<uint> colors, MainWindow mainWindow, int id)
    {
        InitializeComponent();

        _mainWindow = mainWindow;
        _lobbyID = id;
        
        _cypher = ColorTranslator.TranslateUIntToBorder(colors);

        foreach (var border in _cypher)
        {
            _cypherPanel.Children.Add(border);
        }
        
        Update();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _cypherPanel = this.FindControl<WrapPanel>("CypherPanel")!;
        _decryptPanel = this.FindControl<StackPanel>("DecryptPanel")!;

        for (int i = 0; i < 9; i++)
        {
            _decrypt.Add(new List<Border>());
            var decryptWrapPanel = new WrapPanel();

            for (int j = 0; j < 4; j++)
            {
                _decrypt[i].Add(new Border()
                {
                    Height = 50,
                    Width = 50,
                    CornerRadius = CornerRadius.Parse("25"),
                    Background = Brushes.LightGray,
                    BorderThickness = Thickness.Parse("2"),
                    BorderBrush = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
                
                decryptWrapPanel.Children.Add(_decrypt[i][j]);
            }
            
            _decryptPanel.Children.Add(decryptWrapPanel);
        }
    }

    private async void Update()
    {
        while (true)
        {
            await _endPoint.SendAsync(Encoding.UTF8.GetBytes($"STATUS,{_lobbyID}\n"));

            var buffer = new byte[1024];
            await _endPoint.ReceiveAsync(buffer);
            var message = Encoding.UTF8.GetString(buffer).Trim().Split(',');

            if (message[0] == "STATUS")
            {
                var index = int.Parse(message[1]);
                var count = 0;

                foreach (var stringNum in message[2].Split(';'))
                {
                    _decrypt[index][count].Background = ColorTranslator.UIntToBrush(uint.Parse(stringNum));
                    count++;
                }
            }

            if (message[0] == "RESULT")
            {
                var resultWindow = new ResultWindow(message[1], _mainWindow);
                resultWindow.Show();
                Close();
            }
        }
    }
}