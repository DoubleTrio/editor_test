using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
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

    void PrintAllControls(Visual visual, int indent = 0)
    {
        var indentText = new string(' ', indent);
        Console.WriteLine($"{indentText}- {visual.GetType().Name}");

        foreach (var child in visual.GetVisualChildren())
            PrintAllControls(child, indent + 2);
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

    private void MasterTreeView_OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        if (e.Container is TreeViewItem treeViewItem)
        {
            SetContextMenuForTreeItem(treeViewItem);
        }
    }

    private void SetContextMenuForTreeItem(TreeViewItem treeViewItem)
    {
        Console.WriteLine(treeViewItem.DataContext);
        var nodeData = treeViewItem.DataContext;

        var contextMenu = nodeData switch
        {
            PageNode => new ContextMenu
            {
                ItemsSource = new[]
                {
                    new MenuItem { Header = "Close Tab" },
                    new MenuItem { Header = "Close Children" },
                    new MenuItem { Header = "Duplicate" }
                }
            },

            DataRootNode => new ContextMenu
            {
                ItemsSource = new[]
                {
                    new MenuItem { Header = "Add Item" },
                    new MenuItem { Header = "Refresh" },
                    new MenuItem { Header = "Export" }
                }
            },

            _ => null
        };

        treeViewItem.ContextMenu = contextMenu;

        // Recursively set context menus for all children
        foreach (var child in treeViewItem.GetLogicalChildren().OfType<TreeViewItem>())
        {
            SetContextMenuForTreeItem(child);
        }
    }

    private void MasterTreeView_OnContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        if (e.Source is not Visual visual)
            return;

        var current = visual.GetSelfAndVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
        var parent = visual.GetSelfAndVisualAncestors().OfType<TreeViewItem>().Skip(1).FirstOrDefault();

        if (current == null)
            return;

        if (current.DataContext is DataItemNode itemNode &&
            parent?.DataContext is DataRootNode parentRoot)
        {
            ShowDataItemNodeMenu(current, itemNode, parentRoot, e);
        }
        
        else if (current.DataContext is DataItemNode itemNode2 &&
            parent?.DataContext is SpriteRootNode spriteRootMode)
        {
            ShowSpriteItemNodeMenu(current, itemNode2, spriteRootMode, e);
        }
        else if (current.DataContext is DataRootNode rootNode)
        {
            ShowRootNodeMenu(current, rootNode, e);
        } 
        else if (current.DataContext is SpriteRootNode spriteRoot)
        {
            ShowSpriteRootNodeMenu(current, spriteRoot, e);
            
        }
    }

    private void ShowDataItemNodeMenu(TreeViewItem current, DataItemNode node, DataRootNode parentNode,
        ContextRequestedEventArgs e)
    {
        var menu = new ContextMenu
        {
            Items =
            {
                new MenuItem { Header = "Resave as File", Command = parentNode.ResaveAsFile, CommandParameter = node },
                new MenuItem
                    { Header = "Resave as Patch", Command = parentNode.ResaveAsPatch, CommandParameter = node },
                new Separator(),
                new MenuItem { Header = "Edit" },
                new MenuItem { Header = "Delete", Command = parentNode.DeleteCommand, CommandParameter = node }
            }
        };

        AttachAndOpenMenu(current, menu, e);
    }

    private void ShowSpriteItemNodeMenu(TreeViewItem current, DataItemNode node, SpriteRootNode parentNode,
        ContextRequestedEventArgs e)
    {
        var menu = new ContextMenu
        {
            Items =
            {
                new MenuItem { Header = "Import", Command = parentNode.ImportCommand, CommandParameter = node },
                new MenuItem { Header = "Re-Import", Command = parentNode.ReImportCommand, CommandParameter = node },
                new MenuItem { Header = "Export", Command = parentNode.ExportCommand, CommandParameter = node },
                new Separator(),
                new MenuItem { Header = "Delete", Command = parentNode.DeleteCommand, CommandParameter = node }
            }
        };

        AttachAndOpenMenu(current, menu, e);
    }

    private void ShowRootNodeMenu(TreeViewItem current, DataRootNode root, ContextRequestedEventArgs e)
    {
        var menu = new ContextMenu
        {
            Items =
            {
                new MenuItem { Header = "Re-Index", Command = root.ReIndexCommand },
                new MenuItem { Header = "Resave all as File", Command = root.ResaveAllAsFileCommand },
                new MenuItem { Header = "Resave all as Diff", Command = root.ResaveAllAsDiffCommand },
                new Separator(),
                new MenuItem { Header = "Add", Command = root.AddCommand }
            }
        };

        AttachAndOpenMenu(current, menu, e);
    }
    
    private void ShowSpriteRootNodeMenu(TreeViewItem current, SpriteRootNode root, ContextRequestedEventArgs e)
    {
        var menu = new ContextMenu
        {
            Items =
            {
                new MenuItem { Header = "Mass Import", Command = root.MassImportCommand },
                new MenuItem { Header = "Mass Export", Command = root.MassExportCommand },
                new Separator(),
                new MenuItem { Header = "Add", Command = root.AddCommand }
            }
        };

        AttachAndOpenMenu(current, menu, e);
    }

    private static void AttachAndOpenMenu(TreeViewItem current, ContextMenu menu, ContextRequestedEventArgs e)
    {
        menu.Closed += (_, _) => current.ContextMenu = null;
        current.ContextMenu = menu;
        menu.Open(current);
        e.Handled = true;
    }
}