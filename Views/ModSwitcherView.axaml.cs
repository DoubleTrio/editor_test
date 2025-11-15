using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaTest.Models;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;

public partial class ModSwitcherView : UserControl
{
    public ModSwitcherView()
    {
        InitializeComponent();
        ModsListBox.AttachedToVisualTree += (_, _) =>
        {
            
            Dispatcher.UIThread.Post(() =>
            {
                ModsListBox.Focus();
            },  DispatcherPriority.Loaded);
        };
    }
    
    protected override async void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Enter && DataContext is ViewModels.ModSwitcherViewModel switcher)
        {
      
            // switcher.Switch();

            if ((ModsListBox.SelectedItem as ModHeader).Key == switcher._mainWindow.CurrentMod.Key)
            {
                switcher._mainWindow.OnModSwitcherClosed();
                return;
            }
            
            var result = await MessageBoxWindowView.Show(
                "The game will be reloaded to use content from the new path. Click OK to proceed.  Your changes will not be saved.",
                "Are you sure",
                MessageBoxWindowView.MessageBoxButtons.OkCancel,
                switcher._mainWindow._dialogService
            );
            
            if (result == MessageBoxWindowView.MessageBoxResult.Cancel)
                return;
            switcher._mainWindow.CurrentMod = ModsListBox.SelectedItem as ModHeader;
            switcher._mainWindow.OnModSwitcherClosed();
            // switcher._mainWindow._dialogService.ShowDialogAsync<>()
            e.Handled = true;
        }
    }

    private void ModsListBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ModSwitcherViewModel switcher)
            return;
        
        if (e.Key == Key.Down)
        {
            ModsListBox.SelectedIndex = (ModsListBox.SelectedIndex + 1 + ModsListBox.ItemCount) % ModsListBox.ItemCount;
            ModsListBox.Focus(NavigationMethod.Directional);
            e.Handled = true;
        }
        else if (e.Key == Key.Up)
        {
            ModsListBox.SelectedIndex = (ModsListBox.SelectedIndex - 1 + ModsListBox.ItemCount) % ModsListBox.ItemCount;
            ModsListBox.Focus(NavigationMethod.Directional);
            e.Handled = true;
        }
    }
    
    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        
        if (DataContext is ModSwitcherViewModel switcher)
        {
            if ((ModsListBox.SelectedItem as ModHeader).Key == switcher._mainWindow.CurrentMod.Key)
            {
                switcher._mainWindow.OnModSwitcherClosed();
                return;
            }
            
            var result = await MessageBoxWindowView.Show(
                "The game will be reloaded to use content from the new path. Click OK to proceed.  Your changes will not be saved.",
                "Are you sure",
                MessageBoxWindowView.MessageBoxButtons.OkCancel,
                switcher._mainWindow._dialogService
            );
            
            if (result == MessageBoxWindowView.MessageBoxResult.Cancel)
                return;
            switcher._mainWindow.CurrentMod = ModsListBox.SelectedItem as ModHeader;
            switcher._mainWindow.OnModSwitcherClosed();
            // switcher._mainWindow._dialogService.ShowDialogAsync<>()
            e.Handled = true;
 
        }
    }

}