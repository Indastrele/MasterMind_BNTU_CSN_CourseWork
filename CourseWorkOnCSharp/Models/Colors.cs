using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace CourseWorkOnCSharp.Models;

public enum Colors : uint
{ 
    None = 0,
    Red,
    Blue,
    Yellow,
    Green,
    Magenta,
    Purple,
    Black,
    White
}

public static class ColorTranslator
{
    public static List<uint> TranslateToUInt(List<Button> buttons)
    {
        var uints = new List<uint>();

        foreach (var button in buttons)
        {
            if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Red))
            {
                uints.Add((uint)Colors.Red);
            } 
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Blue))
            {
                uints.Add((uint)Colors.Blue);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Yellow))
            {
                uints.Add((uint)Colors.Yellow);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Green))
            {
                uints.Add((uint)Colors.Green);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Magenta))
            {
                uints.Add((uint)Colors.Magenta);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Purple))
            {
                uints.Add((uint)Colors.Purple);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.Black))
            {
                uints.Add((uint)Colors.Black);
            }
            else if (Equals((IImmutableSolidColorBrush?)button.Background, Brushes.White))
            {
                uints.Add((uint)Colors.White);
            }
            else
            {
                uints.Add((uint)Colors.None);
            }
        }

        return uints;
    }

    public static List<Border> TranslateUIntToBorder(List<uint> colors)
    {
        var borders = new List<Border>();

        foreach (var color in colors)
        {
            var border = new Border
            {
                Height = 50,
                Width = 50,
                CornerRadius = CornerRadius.Parse("25"),
                Background = Brushes.LightGray,
                BorderThickness = Thickness.Parse("2"),
                BorderBrush = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            if (color == (uint)Colors.Red)
            {
                border.Background = Brushes.Red;
            }
            else if (color == (uint)Colors.Blue)
            {
                border.Background = Brushes.Blue;
            }
            else if (color == (uint)Colors.Yellow)
            {
                border.Background = Brushes.Yellow;
            }
            else if (color == (uint)Colors.Green)
            {
                border.Background = Brushes.Green;
            }
            else if (color == (uint)Colors.Magenta)
            {
                border.Background = Brushes.Magenta;
            }
            else if (color == (uint)Colors.Purple)
            {
                border.Background = Brushes.Purple;
            }
            else if (color == (uint)Colors.Black)
            {
                border.Background = Brushes.Black;
            }
            else if (color == (uint)Colors.White)
            {
                border.Background = Brushes.White;
            }
            
            borders.Add(border);
        }

        return borders;
    }

    public static IImmutableSolidColorBrush UIntToBrush(uint num)
    {
        return num switch
        {
            (uint)Colors.Red => Brushes.Red,
            (uint)Colors.Blue => Brushes.Blue,
            (uint)Colors.Yellow => Brushes.Yellow,
            (uint)Colors.Green => Brushes.Green,
            (uint)Colors.Magenta => Brushes.Magenta,
            (uint)Colors.Purple => Brushes.Purple,
            (uint)Colors.Black => Brushes.Black,
            (uint)Colors.White => Brushes.White,
            _ => Brushes.LightGray
        };
    }
}