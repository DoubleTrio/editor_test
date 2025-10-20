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

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
        }

        private void ScrollTabs(object _, PointerWheelEventArgs e)
        {
            // if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            // {
            //     if (e.Delta.Y < 0)
            //         LauncherTabsScroller.LineRight();
            //     else if (e.Delta.Y > 0)
            //         LauncherTabsScroller.LineLeft();
            //     e.Handled = true;
            // }
        }

        private void ScrollTabsLeft(object _, RoutedEventArgs e)
        {
            // LauncherTabsScroller.LineLeft();
            e.Handled = true;
        }

        private void ScrollTabsRight(object _, RoutedEventArgs e)
        {
            // LauncherTabsScroller.LineRight();
            // e.Handled = true;
        }

        private void OnTabsLayoutUpdated(object _1, EventArgs _2)
        {
            // SetCurrentValue(IsScrollerVisibleProperty, LauncherTabsScroller.Extent.Width > LauncherTabsScroller.Viewport.Width);
            InvalidateVisual();
        }

        private void OnTabsSelectionChanged(object _1, SelectionChangedEventArgs _2)
        {
            InvalidateVisual();
        }

        private void SetupDragAndDrop(object sender, RoutedEventArgs e)
        {
            if (sender is Border border)
            {
                DragDrop.SetAllowDrop(border, true);
                border.AddHandler(DragDrop.DropEvent, DropTab);
            }
            e.Handled = true;
        }

        private void OnPointerPressedTab(object sender, PointerPressedEventArgs e)
        {
            if (sender is Border border)
            {
                var point = e.GetCurrentPoint(border);
                if (point.Properties.IsMiddleButtonPressed && border.DataContext is ViewModels.EditorPageViewModel page)
                {
                    (DataContext as MainWindowViewModel)?.RemovePage(page);
                    e.Handled = true;
                }
                else
                {
                    _pressedTab = true;
                    _startDragTab = false;
                    _pressedTabPosition = e.GetPosition(border);
                }
            }
        }

        private void OnPointerReleasedTab(object _1, PointerReleasedEventArgs _2)
        {
            _pressedTab = false;
            _startDragTab = false;
        }

        private void OnPointerMovedOverTab(object sender, PointerEventArgs e)
        {
            if (_pressedTab && !_startDragTab && sender is Border { DataContext: ViewModels.EditorPageViewModel page } border)
            {
                var delta = e.GetPosition(border) - _pressedTabPosition;
                var sizeSquired = delta.X * delta.X + delta.Y * delta.Y;
                if (sizeSquired < 64)
                    return;

                _startDragTab = true;

                var data = new DataObject();
                data.Set("MovedTab", page);
                DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
            }
            e.Handled = true;
        }

        private void DropTab(object sender, DragEventArgs e)
        {
            if (e.Data.Contains("MovedTab") &&
                e.Data.Get("MovedTab") is EditorPageViewModel moved &&
                sender is Border { DataContext: EditorPageViewModel to } &&
                to != moved)
            {
                (DataContext as ViewModels.MainWindowViewModel)?.MoveTab(moved, to);
            }

            _pressedTab = false;
            _startDragTab = false;
            e.Handled = true;
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

                //     if (page.Node.IsRepository)
                //     {
                //         var bookmark = new MenuItem();
                //         bookmark.Header = App.Text("PageTabBar.Tab.Bookmark");
                //         bookmark.Icon = App.CreateMenuIcon("Icons.Bookmark");
                //
                //         for (int i = 0; i < Models.Bookmarks.Brushes.Length; i++)
                //         {
                //             var brush = Models.Bookmarks.Brushes[i];
                //             var icon = App.CreateMenuIcon("Icons.Bookmark");
                //             if (brush != null)
                //                 icon.Fill = brush;
                //
                //             var dupIdx = i;
                //             var setter = new MenuItem();
                //             setter.Header = icon;
                //             setter.Click += (_, ev) =>
                //             {
                //                 page.Node.Bookmark = dupIdx;
                //                 ev.Handled = true;
                //             };
                //             bookmark.Items.Add(setter);
                //         }
                //         menu.Items.Add(new MenuItem() { Header = "-" });
                //         menu.Items.Add(bookmark);
                //
                //         var copyPath = new MenuItem();
                //         copyPath.Header = App.Text("PageTabBar.Tab.CopyPath");
                //         copyPath.Icon = App.CreateMenuIcon("Icons.Copy");
                //         copyPath.Click += async (_, ev) =>
                //         {
                //             await page.CopyPathAsync();
                //             ev.Handled = true;
                //         };
                //         menu.Items.Add(new MenuItem() { Header = "-" });
                //         menu.Items.Add(copyPath);
                //     }
                //     menu.Open(border);
                // }
            }

            e.Handled = true;
        }

        private void OnCloseTab(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && DataContext is ViewModels.MainWindowViewModel vm)
                vm.RemovePage(btn.DataContext as ViewModels.EditorPageViewModel);

            e.Handled = true;
        }

        private bool _pressedTab = false;
        private Point _pressedTabPosition = new();
        private bool _startDragTab = false;
    }
}
