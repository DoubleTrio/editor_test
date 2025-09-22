using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace AvaloniaTest.Views;

public partial class CaptionButtonsView : UserControl
{
    public static readonly StyledProperty<bool> IsCloseButtonOnlyProperty =
        AvaloniaProperty.Register<CaptionButtonsView, bool>(nameof(IsCloseButtonOnly));

    public bool IsCloseButtonOnly
    {
        get => GetValue(IsCloseButtonOnlyProperty);
        set => SetValue(IsCloseButtonOnlyProperty, value);
    }

    public CaptionButtonsView()
    {
        InitializeComponent();
    }

    private void MinimizeWindow(object _, RoutedEventArgs e)
    {
        var window = this.FindAncestorOfType<Window>();
        if (window != null)
            window.WindowState = WindowState.Minimized;

        e.Handled = true;
    }

    private void MaximizeOrRestoreWindow(object _, RoutedEventArgs e)
    {
        var window = this.FindAncestorOfType<Window>();
        if (window != null)
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        e.Handled = true;
    }

    private void CloseWindow(object _, RoutedEventArgs e)
    {
        var window = this.FindAncestorOfType<Window>();
        window?.Close();

        e.Handled = true;
    }
}