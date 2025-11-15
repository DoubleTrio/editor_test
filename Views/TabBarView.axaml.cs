using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views
{
    public partial class TabBarView : UserControl
    {
        public TabBarView()
        {
            InitializeComponent();
        }
        
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is MainWindowViewModel vm)
            {
                vm.TabSwitcherClosed += () => TabSwitcherFlyoutButton.Flyout?.Hide();;
            }
        }

        private async void OnCloseTab(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel vm) return;
            if (sender is not Button { DataContext: EditorPageViewModel page }) return;

            if (vm.PageHasChildren(page))
            {
                var res = await MessageBoxWindowView.Show(
                    "Are you sure you want to close all subtabs?  Your changes will not be saved.",
                    "Confirm Close",
                    MessageBoxWindowView.MessageBoxButtons.YesNo,
                    vm._dialogService
                );

                if (res != MessageBoxWindowView.MessageBoxResult.Yes)
                {
                    e.Handled = true;
                    return;
                }
            }

            vm.RemovePage(page);
        }
        
        
        private void TabSwitcherFlyout_OnOpened(object? sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.OpenTabSwitcher();
            }
        }

        private void TabSwitcherFlyout_OnClosed(object? sender, EventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.CloseTabSwitcher();
            }
        }
    }
}