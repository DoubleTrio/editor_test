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
    public ObservableCollection<EditorPageViewModel> VisiblePages { get; } = new ObservableCollection<EditorPageViewModel>();


    private EditorPageViewModel _selectedPage = null;

    public EditorPageViewModel SelectedPage
    {
        get => _selectedPage;
        set { this.RaiseAndSetIfChanged(ref _selectedPage, value); }
    }
    public MainWindowViewModel _mainWindow 
    {
        get;
    }

    private string _searchFilter = string.Empty;

    public string SearchFilter
    {
        get => _searchFilter;
        set { this.RaiseAndSetIfChanged(ref _searchFilter, value); }
    }

    private bool _isTreeView = false;

    public bool IsTreeView
    {
        get => _isTreeView;
        set { this.RaiseAndSetIfChanged(ref _isTreeView, value); }
    }
    
  

    public TabSwitcherViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        UpdateVisiblePages(_searchFilter);

        this.WhenAnyValue(x => x.SearchFilter).Subscribe(UpdateVisiblePages);
        SelectPageCommand = ReactiveCommand.Create<PageNode>(node => 
        {
            _mainWindow.NavigateToPage(node);
        });
        VisiblePages = new ObservableCollection<EditorPageViewModel>(_mainWindow.Pages);
        SelectedPage = _mainWindow.ActivePage;
    }

    public TabSwitcherViewModel() : this(new MainWindowViewModel()) {}
    
    public void ClearFilter()
    {
        SearchFilter = string.Empty;
    }

    
    public void ToggleSearchMode()
    {
        IsTreeView = !IsTreeView;
    }

    public void Switch()
    {
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
        
        UpdateVisiblePagesList(filter);
    }

    private bool ApplyFilterRecursive(PageNode nodeBase, string filter)
    {
        bool match = MatchesFilter(nodeBase.Title, filter);

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
    
    private bool MatchesFilter(string title, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return true;
        
        var tokens = Regex.Split(title, @"[\s_:]+");

        return tokens.Any(token =>
            token.StartsWith(filter, StringComparison.OrdinalIgnoreCase));
    }

    
    private void UpdateVisiblePagesList(string filter)
    {
        VisiblePages.Clear();

        foreach (var page in _mainWindow.Pages)
        {
            if (MatchesFilter(page.Title, filter))
                VisiblePages.Add(page);
        }
        
        SelectedPage = VisiblePages.Count > 0 ? VisiblePages[0] : null;
    }
}