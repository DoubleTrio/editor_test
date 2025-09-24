using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Platform;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;



public partial class MainWindow : ChromelessWindow
{
    public static readonly StyledProperty<GridLength> CaptionHeightProperty =
        AvaloniaProperty.Register<MainWindow, GridLength>(nameof(CaptionHeight));

    public GridLength CaptionHeight
    {
        get => GetValue(CaptionHeightProperty);
        set => SetValue(CaptionHeightProperty, value);
    }

    public static readonly StyledProperty<bool> HasLeftCaptionButtonProperty =
        AvaloniaProperty.Register<MainWindow, bool>(nameof(HasLeftCaptionButton));

    public bool HasLeftCaptionButton
    {
        get => GetValue(HasLeftCaptionButtonProperty);
        set => SetValue(HasLeftCaptionButtonProperty, value);
    }

    public bool HasRightCaptionButton
    {
        get
        {
            if (OperatingSystem.IsLinux())
                return !Native.OS.UseSystemWindowFrame;

            return OperatingSystem.IsWindows();
        }
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

    public MainWindow()
    {
        if (OperatingSystem.IsMacOS())
        {
            HasLeftCaptionButton = true;
            CaptionHeight = new GridLength(34);
            ExtendClientAreaChromeHints =
                ExtendClientAreaChromeHints.SystemChrome | ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
        else if (UseSystemWindowFrame)
        {
            CaptionHeight = new GridLength(30);
        }
        else
        {
            CaptionHeight = new GridLength(38);
        }

        this.DataContextChanged += MainWindow_DataContextChanged;
        InitializeComponent();
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

    private void MasterTreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && sender is TreeView { SelectedItem: not null } treeView)
        {
            var selectedItem = (OpenEditorNode)treeView.SelectedItem;
            
            var editor = vm._pageFactory.CreatePage(selectedItem.EditorKey);


            if (editor != null)
            {
                editor.SetTabInfo(selectedItem);
                vm.AddTopLevelPage(editor);
            }
            // var node = new SpritePageViewModel(vm._tabEvents);
       
            
            // Handle the double-click on the selected item

            // Your logic here
        }
    }
}

