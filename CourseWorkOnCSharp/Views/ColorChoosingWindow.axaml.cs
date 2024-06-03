using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CourseWorkOnCSharp.Views;

public partial class ColorChoosingWindow : Window
{
    private IImmutableSolidColorBrush _buttonColor;

    public IImmutableSolidColorBrush ButtonColor
    {
        get => _buttonColor;
        set => _buttonColor = value;
    }

    public ColorChoosingWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        _buttonColor = Brushes.LightGray;
    }
    
    public ColorChoosingWindow(IImmutableSolidColorBrush color)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        _buttonColor = color;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    public void Color_Click(object sender, KeyEventArgs e)
    {
        _buttonColor = (sender as ListBoxItem).Foreground as IImmutableSolidColorBrush;
        
        Close();
    }
}