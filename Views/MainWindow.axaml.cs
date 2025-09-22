using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent(); // TODO: There has to be a better way of hiding the flyout...
        this.DataContextChanged += MainWindow_DataContextChanged;

    }
    private void MainWindow_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.TabSwitcherClosed += OnTabSwitcherClosed;
        }
    }
    
    private void OnTabSwitcherClosed(object? sender, EventArgs e)
    {
   
                TabSwitcherFlyoutButton.Flyout?.Hide();
    
    }
    

    // public void ButtonClicked(object source, RoutedEventArgs args)
    // {
    //     if (Double.TryParse(celsius.Text, out double C))
    //     {
    //         var F = C * (9d / 5d) + 32;
    //         fahrenheit.Text = F.ToString("0.0");
    //     }
    //     else
    //     {
    //         celsius.Text = "0";
    //         fahrenheit.Text = "0";
    //     }
    // }
    //
    //
    // private void Celsius_OnKeyDown(object? sender, KeyEventArgs e)
    // {
    //     Debug.WriteLine(e.Key);
    //     if (double.TryParse(celsius.Text, out double C))
    //     {
    //         Debug.WriteLine($"Celsius: {C}");
    //         var F = C * (9d / 5d) + 32;
    //         fahrenheit.Text = F.ToString("0.00");
    //     }
    //     else
    //     {
    //         celsius.Text = "0";
    //         fahrenheit.Text = "0";
    //     }
    // }
    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var results = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            // Title = "Open .rsground File",
            // SuggestedStartLocation = await form.GroundEditForm.StorageProvider.TryGetFolderFromPathAsync(mapDir),
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Ground Files")
                {
                    Patterns = new[] { "*." +　"json" }
                }
            }
        });

        if (results != null && results.Count > 0)
        {
            var selected = results[0];
            Console.WriteLine(selected.Path.AbsolutePath);

        }
    }
    private void FlyoutBase_OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.OnTabSwitcherOpened();
        }
    }

    private void FlyoutBase_OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.OnTabSwitcherClosed();
        }
    }
    
}