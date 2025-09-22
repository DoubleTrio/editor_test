using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using AvaloniaTest.Services;
using AvaloniaTest.Views;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

using Avalonia.Collections;
using Avalonia.Threading;



public static class TabEvents
{
    // Event for adding child tabs
    public static event Action<EditorPageViewModel, EditorPageViewModel> AddChildTabRequested;
    
    // Generic method to create and request adding a child tab
    public static void RequestAddChildTab<T>(EditorPageViewModel parent) where T : EditorPageViewModel, new()
    {
        var child = new T();
        AddChildTabRequested?.Invoke(parent, child);
    }
    
    // Optional: Method with custom title
    public static void RequestAddChildTab<T>(EditorPageViewModel parent, string title) where T : EditorPageViewModel, new()
    {
        var child = new T();
        child.Title = title; // Assuming EditorPageViewModel has a Title property
        AddChildTabRequested?.Invoke(parent, child);
    }
    
    // Optional: Method to add pre-created child
    public static void RequestAddChildTab(EditorPageViewModel parent, EditorPageViewModel child)
    {
        AddChildTabRequested?.Invoke(parent, child);
    }
}


public class MainWindowViewModel : ViewModelBase
{
    public void OnTabSwitcherOpened()
    {
        if (TabSwitcher == null)
        {
            TabSwitcher = new TabSwitcherViewModel(this);
            Console.WriteLine("TabSwitcher created");
        }
    }

    public void OnTabSwitcherClosed()
    {
        if (TabSwitcher != null)
        {
            TabSwitcher.Dispose();
            TabSwitcher = null;
            Console.WriteLine("TabSwitcher disposed");
        }
    }


    public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; }

    public ReactiveCommand<Unit, Unit> AddDevControlTab { get; }
    private string _filter = "";


    public string Filter
    {
        get { return _filter; }
        set { this.RaiseAndSetIfChanged(ref _filter, value); }
    }


    private ObservableCollection<EditorPageViewModel> _tabNodes;

    public ObservableCollection<EditorPageViewModel> TabNodes
    {
        get => _pages;
        set => this.RaiseAndSetIfChanged(ref _pages, value);
    }


    private ObservableCollection<EditorPageViewModel> _pages;

    public ObservableCollection<EditorPageViewModel> Pages
    {
        get => _pages;
        set => this.RaiseAndSetIfChanged(ref _pages, value);
    }

    private EditorPageViewModel _activePage;

    public EditorPageViewModel ActivePage
    {
        get => _activePage;
        set => this.RaiseAndSetIfChanged(ref _activePage, value);
    }

    
    private ObservableCollection<PageNode> _topLevelPages;
    public ObservableCollection<PageNode> TopLevelPages
    {
        get => _topLevelPages;
        set => this.RaiseAndSetIfChanged(ref _topLevelPages, value);
    }
    
    private Dictionary<EditorPageViewModel, PageNode> _pageToNodeMap;

    
    private void OnItemSelected()
    {
        // Request flyout closure through an event or action
        TabSwitcherItemSelected?.Invoke();
    }

    public event Action TabSwitcherItemSelected;

    // public IDisposable Switcher
    // {
    //     get => _switcher;
    //     private set => SetProperty(ref _switcher, value);
    // }
    //
    
    public void AddTopLevelPage(EditorPageViewModel page)
    {
        Pages.Add(page);
        
        var node = new PageNode(page);
        TopLevelPages.Add(node);
        _pageToNodeMap[page] = node;
    }
    
    public void AddChildPage(EditorPageViewModel parentPage, EditorPageViewModel childPage)
    {
        Pages.Add(childPage);
        
        if (_pageToNodeMap.TryGetValue(parentPage, out var parentNode))
        {
            var childNode = parentNode.AddChild(childPage);
            _pageToNodeMap[childPage] = childNode;
        }
        else
        {
            // Parent not found, add as top-level
            AddTopLevelPage(childPage);
        }
    }
    
    // Remove a page and handle hierarchy
    public void RemovePage(EditorPageViewModel page)
    {
        if (!_pageToNodeMap.TryGetValue(page, out var node))
            return;
            
        // Recursively close all children first
        ClosePageAndChildren(node);
    }

    private void ClosePageAndChildren(PageNode node)
    {
        // Close all children recursively
        var children = node.SubNodes.Cast<PageNode>().ToList();
        foreach (var child in children)
        {
            ClosePageAndChildren(child);
        }
        
        // Remove from flat Pages collection
        Pages.Remove(node.Page);
        
        // Remove from hierarchy
        if (node.IsTopLevel)
        {
            TopLevelPages.Remove(node);
        }
        else
        {
            node.Parent.RemoveChild(node);
        }
        
        // Clean up mapping
        _pageToNodeMap.Remove(node.Page);
        
        // Dispose if page implements IDisposable
        if (node.Page is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    
    
    
    
    public MainWindowViewModel()
    {
        
        TabEvents.AddChildTabRequested += (parent, child) =>
        {
            AddChildPage(parent, child);
            ActivePage = child;
        };
        
        // TreeModel = new TreeSearchViewModel();
        // var service = new ToDoService();
        // ToDoList = new ToDoListViewModel(service.GetItems());
        // _contentViewModel = ToDoList;

        // RoleBinder = new AutoCompleteBinder<string>(Roles, (_) => { });

        // TODO: Add dev page here!
        Pages = new ObservableCollection<EditorPageViewModel>();
        Pages.Add(new DevControlViewModel());
        
        TopLevelPages = new ObservableCollection<PageNode>();
        _pageToNodeMap = new Dictionary<EditorPageViewModel, PageNode>();
        
        TreeSearch = new TreeSearchViewModel();
        ClearFilterCommand = ReactiveCommand.Create(() => { Filter = string.Empty; });

        AddDevControlTab = ReactiveCommand.Create(() =>
        {
            var page = new SpritePageViewModel();
            Pages.Add(page);
            ActivePage = page;
        });

        OpenTabSwitcher = ReactiveCommand.Create(() =>
        {
            var vm = new TabSwitcherViewModel(this);
            TabSwitcher = vm;
            Console.WriteLine("Test?");
            return vm;
        });

        CloseTabSwitcher = ReactiveCommand.Create(() =>
        {
            _switcher?.Dispose();
            TabSwitcher = null;
            return Unit.Default;
        });

        this.WhenAnyValue(x => x.Filter).Subscribe(ApplyFilter);
    }

    private void ApplyFilter(string filter)
    {
        foreach (var node in TreeSearch.Nodes)
        {
            ApplyFilterRecursive(node, filter);
        }
    }

    private bool ApplyFilterRecursive(NodeBase nodeBase, string filter)
    {
        bool match = string.IsNullOrWhiteSpace(filter);

        // Split by any combination of spaces, underscores, and colons. Might need to adjust later. 
        var tokens = Regex.Split(nodeBase.
            Title, @"[\s_:]+");

        match |= tokens.Any(token =>
            token.StartsWith(filter, StringComparison.OrdinalIgnoreCase));

        bool childMatch = false;
        foreach (var child in nodeBase.SubNodes)
        {
            if (ApplyFilterRecursive(child, filter))
                childMatch = true;
        }

        nodeBase.IsVisible = match || childMatch;

        if (!string.IsNullOrWhiteSpace(filter))
            nodeBase.IsExpanded = childMatch;

        return nodeBase.IsVisible;
    }


    public void AddNewTab()
    {
        var page = new DevControlViewModel();
        Pages.Add(page);
        ActivePage = page;
    }

    private bool _ignoreIndexChange = false;

    public void MoveTab(EditorPageViewModel from, EditorPageViewModel to)
    {
        _ignoreIndexChange = true;

        var fromIdx = Pages.IndexOf(from);
        var toIdx = Pages.IndexOf(to);
        Pages.Move(fromIdx, toIdx);
        ActivePage = from;

        // ActiveWorkspace.Repositories.Clear();
        // foreach (var p in Pages)
        // {
        //     if (p.Data is Repository r)
        //         ActiveWorkspace.Repositories.Add(r.FullPath);
        // }
        // ActiveWorkspace.ActiveIdx = ActiveWorkspace.Repositories.IndexOf(from.Node.Id);

        _ignoreIndexChange = false;
    }

    public void GotoNextTab()
    {
        if (Pages.Count == 1)
            return;

        var activeIdx = Pages.IndexOf(_activePage);
        var nextIdx = (activeIdx + 1) % Pages.Count;
        ActivePage = Pages[nextIdx];
    }

    public ReactiveCommand<Unit, TabSwitcherViewModel> OpenTabSwitcher { get; }
    public ReactiveCommand<Unit, Unit> CloseTabSwitcher { get; }

    public void GotoPrevTab()
    {
        if (Pages.Count == 1)
            return;

        var activeIdx = Pages.IndexOf(_activePage);
        var prevIdx = activeIdx == 0 ? Pages.Count - 1 : activeIdx - 1;
        ActivePage = Pages[prevIdx];
    }

    public void CloseTab(EditorPageViewModel page)
    {
        if (Pages.Count == 1)
        {
            var last = Pages[0];
            if (true)
                // if (last.Data is Repository repo)
            {
                // ActiveWorkspace.Repositories.Clear();
                // ActiveWorkspace.ActiveIdx = 0;
                //
                // repo.Close();
                //
                // Welcome.Instance.ClearSearchFilter();
                // last.Node = new RepositoryNode() { Id = Guid.NewGuid().ToString() };
                // last.Data = Welcome.Instance;
                // last.Popup?.Cleanup();
                // last.Popup = null;
                // UpdateTitle();
                //
                // GC.Collect();
            }
            else
            {
                Console.WriteLine("QUIT NOW!");
                // App.Quit(0);
            }

            return;
        }

        page ??= _activePage;

        var removeIdx = Pages.IndexOf(page);
        var activeIdx = Pages.IndexOf(_activePage);
        if (removeIdx == activeIdx)
            ActivePage = Pages[removeIdx > 0 ? removeIdx - 1 : removeIdx + 1];

        // CloseRepositoryInTab(page);
        Pages.RemoveAt(removeIdx);
        // GC.Collect();
    }

    public void CloseOtherTabs()
    {
        if (Pages.Count == 1)
            return;

        _ignoreIndexChange = true;

        // var id = ActivePage.Node.Id;
        // foreach (var one in Pages)
        // {
        //     if (one.Node.Id != id)
        //         CloseRepositoryInTab(one);
        // }
        //

        // TODO: title is not accurate at all...
        var id = ActivePage.Title;
        foreach (var one in Pages)
        {
            //     if (one.Node.Id != id)
            //         CloseRepositoryInTab(one);
        }

        Pages = new ObservableCollection<EditorPageViewModel>() { ActivePage };
        // ActiveWorkspace.ActiveIdx = 0;
        // OnPropertyChanged(nameof(Pages));

        _ignoreIndexChange = false;
        GC.Collect();
    }

    public void CloseRightTabs()
    {
        _ignoreIndexChange = true;

        var endIdx = Pages.IndexOf(ActivePage);
        for (var i = Pages.Count - 1; i > endIdx; i--)
        {
            // CloseRepositoryInTab(Pages[i]);
            Pages.Remove(Pages[i]);
        }

        _ignoreIndexChange = false;
        GC.Collect();
    }

    private IDisposable? _switcher = null;

    public event EventHandler? TabSwitcherClosed;

    public IDisposable? TabSwitcher
    {
        get => _switcher;
        set
        {
            this.RaiseAndSetIfChanged(ref _switcher, value);
            if (value == null)
            {
                Console.WriteLine("A");
                TabSwitcherClosed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void CancelSwitcher()
    {
        OnTabSwitcherClosed();
        // IsSwitcherOpen = false; // This will handle disposal automatically
    }

    // Navigate to a page from tab switcher
    public void NavigateToPage(PageNode node)
    {
        ActivePage = node.Page;
        // Close tab switcher
        // TabSwitcher.IsSwitcherOpen = false;
    }
    
    // Find node by page
    // public PageNode FindNode(EditorPageViewModel page)
    // {
    //     return _pageToNodeMap.TryGetValue(page, out var node) ? node : null;
    // }

    public TreeSearchViewModel TreeSearch { get; } = new TreeSearchViewModel();
    // public TabSwitcherViewModel TabSwitcher { get; }
}

