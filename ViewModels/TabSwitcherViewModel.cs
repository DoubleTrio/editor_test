using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using Avalonia.Media;
using AvaloniaTest.Utility;
using DynamicData;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

public class TabSwitcherViewModel: ViewModelBase
{
    public ObservableCollection<EditorPageViewModel> VisiblePages { get; } = new ObservableCollection<EditorPageViewModel>();
    
    private EditorPageViewModel? _selectedPage;

    public EditorPageViewModel? SelectedPage
    {
        get => _selectedPage;
        set => this.RaiseAndSetIfChanged(ref _selectedPage, value);
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

    
    public TabSwitcherViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        UpdateVisiblePages(_searchFilter);
        
        VisiblePages = new ObservableCollection<EditorPageViewModel>(_mainWindow.Pages);
        SelectedPage = _mainWindow.ActivePage;
        this.WhenAnyValue(x => x.SearchFilter).Subscribe(UpdateVisiblePages);
    }

    public TabSwitcherViewModel() : this(new MainWindowViewModel()) {}
    
    public void ClearFilter()
    {
        SearchFilter = string.Empty;
    }

    
    public void ToggleSearchMode()
    {
        _mainWindow.IsTreeView = !_mainWindow.IsTreeView;
    }

    public void Switch()
    {
        _mainWindow.ActivePage = _selectedPage;
        _mainWindow.CloseTabSwitcher();
    }
    
    // Do it for both the tab list and the tab tree view!
    private void UpdateVisiblePages(string filter)
    {
        foreach (var node in _mainWindow.TopLevelPages)
        {
            NodeHelper.FilterRecursive(node, filter);
        }
        
        UpdateVisiblePagesList(filter);
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