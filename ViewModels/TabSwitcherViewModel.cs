using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using DynamicData;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class TabSwitcherViewModel: ViewModelBase, IDisposable
{
    public ObservableCollection<EditorPageViewModel> VisiblePages { get; set; } =
        new ObservableCollection<EditorPageViewModel>();
    
    public MainWindowViewModel _mainWindow 
    {
        get;
    }

    private EditorPageViewModel _selectedPage = null;
    
    private string _searchFilter = string.Empty;

    public string SearchFilter
    {
        get => _searchFilter;
        set { this.RaiseAndSetIfChanged(ref _searchFilter, value); }
    }
    
    public EditorPageViewModel SelectedPage
    {
        get => _selectedPage;
        set { this.RaiseAndSetIfChanged(ref _selectedPage, value); }
    }

    public TabSwitcherViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        UpdateVisiblePages(_searchFilter);
        SelectedPage = _mainWindow.ActivePage;
        this.WhenAnyValue(x => x.SearchFilter).Subscribe(UpdateVisiblePages);
        SelectPageCommand = ReactiveCommand.Create<PageNode>(node => 
        {
            _mainWindow.NavigateToPage(node);
        });
    }

    
    public TabSwitcherViewModel()
    {
        _mainWindow = new MainWindowViewModel();
        UpdateVisiblePages(_searchFilter);
        SelectedPage = _mainWindow.ActivePage;
        this.WhenAnyValue(x => x.SearchFilter).Subscribe(UpdateVisiblePages);
        SelectPageCommand = ReactiveCommand.Create<PageNode>(node => 
        {
            _mainWindow.NavigateToPage(node);
        });
    }
    public void ClearFilter()
    {
        SearchFilter = string.Empty;
    }

    public void Switch()
    {
        Console.WriteLine(_selectedPage + "NEW");
        _mainWindow.ActivePage = _selectedPage ?? _mainWindow.ActivePage;
        _mainWindow.CancelSwitcher();
    }

    public void Dispose()
    {
        // VisiblePages.Clear();
        // _selectedPage = null;
        // _searchFilter = string.Empty;
    }

    public ReactiveCommand<PageNode, Unit> SelectPageCommand { get; }
    
    
    private void UpdateVisiblePages(string filter)
    {
        foreach (var node in _mainWindow.TopLevelPages)
        {
            ApplyFilterRecursive(node, filter);
        }
    }

    private bool ApplyFilterRecursive(PageNode nodeBase, string filter)
    {
        bool match = string.IsNullOrWhiteSpace(filter);

        // Split by any combination of spaces, underscores, and colons. Might need to adjust later. 
        var tokens = Regex.Split(nodeBase.
            Title, @"[\s_:]+");

        match |= tokens.Any(token =>
            token.StartsWith(filter, StringComparison.OrdinalIgnoreCase));

        bool childMatch = false;
        foreach (PageNode child in nodeBase.SubNodes)
        {
            if (ApplyFilterRecursive(child, filter))
                childMatch = true;
        }

        nodeBase.IsVisible = match || childMatch;

        if (!string.IsNullOrWhiteSpace(filter))
            nodeBase.IsExpanded = childMatch;

        return nodeBase.IsVisible;
    }
    
    // private void UpdateVisiblePages()
    // {
        
        
        // VisiblePages.Clear();
        //
        // if (string.IsNullOrWhiteSpace(_searchFilter))
        // {
        //     foreach (var page in _mainWindow.Pages)
        //         VisiblePages.Add(page);
        // }
        // else
        // {
        //     foreach (var page in _mainWindow.Pages)
        //     {
        //         if (page.Title.StartsWith(_searchFilter, StringComparison.OrdinalIgnoreCase))
        //             VisiblePages.Add(page);
        //     }
        //     
        //     foreach (var page in _mainWindow.Pages)
        //     {
        //         if (!page.Title.StartsWith(_searchFilter, StringComparison.OrdinalIgnoreCase) &&
        //             page.Title.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase))
        //         {
        //             VisiblePages.Add(page);
        //         }
        //     }
        // }
        //
        // SelectedPage = VisiblePages.Count > 0 ? VisiblePages[0] : null;
    // }


}