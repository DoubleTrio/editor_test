using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views;

public partial class TabSwitcherView : UserControl
{

    public TabSwitcherView()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Enter && DataContext is ViewModels.TabSwitcherViewModel switcher)
        {
            switcher.Switch();
            e.Handled = true;
        }
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (DataContext is ViewModels.TabSwitcherViewModel switcher)
        {
            switcher.Switch();
            e.Handled = true;
        }
    }

    private void OnSearchBoxKeyDown(object sender, KeyEventArgs e)
    {
    }
    //     if (PagesListBox.ItemCount == 0)
    //         return;
    //
    //     if (e.Key == Key.Down)
    //     {
    //         PagesListBox.Focus(NavigationMethod.Directional);
    //
    //         if (PagesListBox.SelectedIndex < PagesListBox.ItemCount - 1)
    //             PagesListBox.SelectedIndex++;
    //         else
    //             PagesListBox.SelectedIndex = 0;
    //
    //         e.Handled = true;
    //     }
    //     else if (e.Key == Key.Up)
    //     {
    //         PagesListBox.Focus(NavigationMethod.Directional);
    //
    //         if (PagesListBox.SelectedIndex > 0)
    //             PagesListBox.SelectedIndex--;
    //         else
    //             PagesListBox.SelectedIndex = PagesListBox.ItemCount - 1;
    //
    //         e.Handled = true;
    //     }
    // }
    private void TreeViewTabSwitcher_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("PRESSED ITEM??");
        if (DataContext is ViewModels.TabSwitcherViewModel switcher)
        {
            switcher.Switch();
            e.Handled = true;
        }
    }

    private void TreeViewTabSwitcher_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Console.WriteLine("ENTERED ITEM??");
    }
    

    private void TreeViewTabSwitcher_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is TreeView tree && e.AddedItems.Count > 0)
        {
            if (e.AddedItems[0] is PageNode selectedNode)
            {
                Console.WriteLine($"Selected PageNode: {selectedNode.Title}");

                
                if (DataContext is ViewModels.TabSwitcherViewModel switcher)
                {
                    switcher._mainWindow.ActivePage = selectedNode.Page;
                    // switcher.Switch(selectedNode);
                    switcher._mainWindow.TabSwitcher = null;
                    // switcher._mainWindow.CancelSwitcher();
                }
                
                e.Handled = true;
            }
        }
    }
}