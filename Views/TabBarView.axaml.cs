using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views
{
    public partial class TabBarView : UserControl
    {
        public static readonly StyledProperty<bool> IsScrollerVisibleProperty =
            AvaloniaProperty.Register<TabBarView, bool>(nameof(IsScrollerVisible));

        public bool IsScrollerVisible
        {
            get => GetValue(IsScrollerVisibleProperty);
            set => SetValue(IsScrollerVisibleProperty, value);
        }

        public TabBarView()
        {
            InitializeComponent();
        }

        private void OnTabContextRequested(object sender, ContextRequestedEventArgs e)
        {
            if (sender is Border { DataContext: ViewModels.EditorPageViewModel page } border &&
                DataContext is ViewModels.MainWindowViewModel vm)
            {
                var menu = new ContextMenu();
                var close = new MenuItem();
                // close.Header = App.Text("PageTabBar.Tab.Close");
                close.Header = "Close";
                close.Tag = OperatingSystem.IsMacOS() ? "⌘+W" : "Ctrl+W";
                close.Click += (_, ev) =>
                {
                    vm.RemovePage(page);
                    ev.Handled = true;
                };
                menu.Items.Add(close);

                var closeOthers = new MenuItem();
                // closeOthers.Header = App.Text("PageTabBar.Tab.CloseOther");
                closeOthers.Header = "Click Others";
                closeOthers.Click += (_, ev) =>
                {
                    vm.CloseOtherTabs();
                    ev.Handled = true;
                };
                menu.Items.Add(closeOthers);

                var closeRight = new MenuItem();
                // closeRight.Header = App.Text("PageTabBar.Tab.CloseRight");
                closeRight.Header = "Close Right";

                closeRight.Click += (_, ev) =>
                {
                    vm.CloseRightTabs();
                    ev.Handled = true;
                };
                menu.Items.Add(closeRight);
            }

            e.Handled = true;
        }

        private async void OnCloseTab(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && DataContext is ViewModels.MainWindowViewModel vm)
                {
                    if (btn.DataContext is ViewModels.EditorPageViewModel page)
                    {
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

                        Console.WriteLine($"Closing tab: {page.Title}");
                        vm.RemovePage(page);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or display the error — this prevents a crash
                Console.Error.WriteLine($"Error closing tab: {ex}");
                await MessageBoxWindowView.Show(
                    $"An error occurred while closing the tab:\n\n{ex.Message}",
                    "Error",
                    MessageBoxWindowView.MessageBoxButtons.Ok,
                    (DataContext as ViewModels.MainWindowViewModel)?._dialogService
                );
            }
            finally
            {
                e.Handled = true;
            }
        }
    }
}
