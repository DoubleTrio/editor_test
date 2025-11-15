using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using AvaloniaTest.Models;
using AvaloniaTest.Services;
using AvaloniaTest.Utility;
using AvaloniaTest.Views;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

using Avalonia.Collections;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Media;

public class MainWindowViewModel : ViewModelBase
{
    private readonly NodeFactory _nodeFactory;


    private bool _isTreeView = false;

    public bool IsTreeView
    {
        get => _isTreeView;
        set { this.RaiseAndSetIfChanged(ref _isTreeView, value); }
    }


    private ModHeader _currentMod = new ModHeader("Halcyon", "halcyon");

    public ModHeader CurrentMod
    {
        get => _currentMod;
        set { this.RaiseAndSetIfChanged(ref _currentMod, value); }
    }

    public void OpenTabSwitcher()
    {
        TabSwitcher = new TabSwitcherViewModel(this);
    }
    public event Action? TabSwitcherClosed;
    public void CloseTabSwitcher()
    {
        TabSwitcher = null;
        TabSwitcherClosed?.Invoke();
    }

    public void OnModSwitcherOpened()
    {
        if (ModSwitcher == null)
        {
            ModSwitcher = new ModSwitcherViewModel(this);
        }
    }

    public void OnModSwitcherClosed()
    {
        if (ModSwitcher != null)
        {
            ModSwitcher = null;
        }
    }

    public ReactiveCommand<Unit, Unit> OpenPreferencesWindow { get; }

    public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; }

    public ReactiveCommand<Unit, Unit> AddTestTab { get; }
    private string _filter = "";


    public string Filter
    {
        get { return _filter; }
        set { this.RaiseAndSetIfChanged(ref _filter, value); }
    }


    private ObservableCollection<EditorPageViewModel> _pages;

    public ObservableCollection<EditorPageViewModel> Pages
    {
        get => _pages;
        set => this.RaiseAndSetIfChanged(ref _pages, value);
    }

    private EditorPageViewModel? _activePage;

    public EditorPageViewModel? ActivePage
    {
        get => _activePage;
        set => this.RaiseAndSetIfChanged(ref _activePage, value);
    }

    private EditorPageViewModel? _temporaryTab;


    public EditorPageViewModel? TemporaryTab
    {
        get => _temporaryTab;
        set => this.RaiseAndSetIfChanged(ref _temporaryTab, value);
    }

    private ObservableCollection<PageNode> _topLevelPages;

    public ObservableCollection<PageNode> TopLevelPages
    {
        get => _topLevelPages;
        set => this.RaiseAndSetIfChanged(ref _topLevelPages, value);
    }

    private Dictionary<EditorPageViewModel, PageNode> _pageToNodeMap;


    public void AddTopLevelPage(EditorPageViewModel page)
    {
        Console.WriteLine($"Adding top level page {page}");
        // Navigate to the tab if it already exists
        if (page.UniqueId != null)
        {
            var existing = Pages.FirstOrDefault(p => p.UniqueId == page.UniqueId);
            if (existing != null)
            {
                ActivePage = existing;
                TemporaryTab = null;
                Console.WriteLine($"Already exists, navigating to {existing}");
                return;
            }
        }

        // The tab doesn't add a new page
        if (!page.AddNewTab)
        {
            TemporaryTab = page;
            return;
        }

        Pages.Add(page);

        var node = new PageNode(_dialogService, page);
        TopLevelPages.Add(node);
        _pageToNodeMap[page] = node;
        ActivePage = page;
    }


    public void AddTopLevelPage(EditorPageViewModel parentPage, EditorPageViewModel childPage)
    {
        // Prevent duplicates
        if (childPage.UniqueId != null)
        {
            var existing = Pages.FirstOrDefault(p => p.UniqueId == childPage.UniqueId);
            if (existing != null)
            {
                ActivePage = existing;
                return;
            }
        }

        // Find parent index
        var parentIndex = Pages.IndexOf(parentPage);
        if (parentIndex == -1)
        {
            // Fallback: if parent isn't found, just add to the end
            Pages.Add(childPage);
            var fallbackNode = new PageNode(_dialogService, childPage);
            TopLevelPages.Add(fallbackNode);
            _pageToNodeMap[childPage] = fallbackNode;
            ActivePage = childPage;
            return;
        }

        // Insert the child right after the parent
        var insertIndex = parentIndex + 1;
        Pages.Insert(insertIndex, childPage);

        // Create and insert the node at the same position
        var childNode = new PageNode(_dialogService, childPage);
        var parentNode = _pageToNodeMap.TryGetValue(parentPage, out var pNode) ? pNode : null;
        if (parentNode != null)
        {
            var parentNodeIndex = TopLevelPages.IndexOf(parentNode);
            if (parentNodeIndex != -1)
            {
                TopLevelPages.Insert(parentNodeIndex + 1, childNode);
            }
            else
            {
                TopLevelPages.Add(childNode);
            }
        }
        else
        {
            TopLevelPages.Insert(insertIndex, childNode);
        }

        _pageToNodeMap[childPage] = childNode;
        ActivePage = childPage;
    }


    public void AddChildPage(EditorPageViewModel parentPage, EditorPageViewModel childPage)
    {
        if (childPage.UniqueId != null)
        {
            var existing = Pages.FirstOrDefault(p => p.UniqueId == childPage.UniqueId);
            if (existing != null)
            {
                TemporaryTab = null;
                ActivePage = existing;
                return;
            }
        }

        if (_pageToNodeMap.TryGetValue(parentPage, out var parentNode))
        {
            var childNode = parentNode.AddChild(_dialogService, childPage);
            _pageToNodeMap[childPage] = childNode;
            var parentIndex = Pages.IndexOf(parentPage);
            if (parentIndex == -1)
            {
                Pages.Add(childPage);
            }
            else
            {
                Pages.Insert(parentIndex + 1, childPage);
            }
        }
        else
        {
            AddTopLevelPage(parentPage);
        }
    }

    public bool PageHasChildren(EditorPageViewModel page)
    {
        if (!_pageToNodeMap.TryGetValue(page, out var node))
            return false;

        return node.SubNodes.Count > 0;
    }

    public void RemovePage(EditorPageViewModel page)
    {
        if (!_pageToNodeMap.TryGetValue(page, out var node))
            return;

        bool wasActive = (page == _activePage);
        int removeIdx = Pages.IndexOf(page);

        ClosePageAndChildren(node);

        if (wasActive)
        {
            if (Pages.Count == 0)
            {
                ActivePage = null; // no pages left
            }
            else if (removeIdx < Pages.Count)
            {
                ActivePage = Pages[removeIdx];
            }
            else
            {
                ActivePage = Pages[Pages.Count - 1];
            }
        }
    }

    private void ClosePageAndChildren(PageNode node)
    {
        var children = node.SubNodes.Cast<PageNode>().ToList();
        foreach (var child in children)
        {
            ClosePageAndChildren(child);
        }

        Pages.Remove(node.Page);

        if (node.IsTopLevel)
        {
            TopLevelPages.Remove(node);
        }
        else
        {
            node.Parent.RemoveChild(node);
        }

        _pageToNodeMap.Remove(node.Page);

        if (node.Page is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }


    public readonly PageFactory _pageFactory;
    public readonly TabEvents _tabEvents;
    public readonly IDialogService _dialogService;
    public ObservableCollection<NodeBase> Nodes { get; set; }

    public MainWindowViewModel() : this(new PageFactory(new DesignServiceProvider()),
        new NodeFactory(new DesignServiceProvider()), new DialogService(),
        new TabEvents(new PageFactory(new DesignServiceProvider())))
    {
    }

    public MainWindowViewModel(PageFactory pageFactory, NodeFactory nodeFactory, IDialogService dialogService,
        TabEvents tabEvents)
    {
        _pageFactory = pageFactory;
        _tabEvents = tabEvents;
        _nodeFactory = nodeFactory;
        _dialogService = dialogService;

        BuildNodes();
        InitializeTabEvents();

        this.WhenAnyValue(x => x.ActivePage)
            .Where(activePage => activePage != null)
            .Subscribe(_ => TemporaryTab = null);

        Pages = new ObservableCollection<EditorPageViewModel>();
        TopLevelPages = new ObservableCollection<PageNode>();
        _pageToNodeMap = new Dictionary<EditorPageViewModel, PageNode>();

        TreeSearch = new TreeSearchViewModel();

        // TODO: move this own view
        ClearFilterCommand = ReactiveCommand.Create(() => { Filter = string.Empty; });

        AddTestTab = ReactiveCommand.Create(() =>
        {
            var page = _pageFactory.CreatePage("SpritePage");

            _pageFactory.PrintRegisteredPages();

            if (page != null)
            {
                AddTopLevelPage(page);
            }

            ActivePage = page;
        });


        OpenPreferencesWindow = ReactiveCommand.CreateFromTask(async () =>
        {
            await _dialogService.ShowDialogAsync<PreferencesWindowViewModel, bool>(
                PreferencesWindowViewModel.Instance, "Preferences", false);
        });

        var tab = _pageFactory.CreatePage("DevControl");
        tab.Icon = "Icons.GameControllerFill";
        AddTopLevelPage(tab);
        this.WhenAnyValue(x => x.Filter).Subscribe(ApplyFilter);
    }

    private void InitializeTabEvents()
    {
        _tabEvents.AddChildTabEvent += (parent, child) =>
        {
            AddChildPage(parent, child);
            ActivePage = child;
        };

        _tabEvents.AddTopLevelTabEvent += (tab) =>
        {
            AddTopLevelPage(tab);
            ActivePage = tab;
        };

        _tabEvents.AddTemporaryTabEvent += (tab) =>
        {
            TemporaryTab = tab;
            ActivePage = null;
        };

        _tabEvents.RemoveTabEvent += (tab) => { RemovePage(tab); };
        
        _tabEvents.NavigateToTabEvent += (tab) =>
        {
            ActivePage = tab;
        };
    }

    private void BuildNodes()
    {
        var halcyonNode = _nodeFactory.CreateOpenEditorNode("Halcyon", "Icons.FloppyDiskBackFill", "ModInfoEditor");

        halcyonNode.SubNodes.Add(
            _nodeFactory.CreateOpenEditorNode("Dev Control", "Icons.GameControllerFill", "DevControl"));
        halcyonNode.SubNodes.Add(_nodeFactory.CreateOpenEditorNode("Zone Editor", "Icons.StairsFill", "ZoneEditor"));
        halcyonNode.SubNodes.Add(
            _nodeFactory.CreateOpenEditorNode("Ground Editor", "Icons.MapTrifoldFill", "GroundEditor"));
        halcyonNode.SubNodes.Add(_nodeFactory.CreateOpenEditorNode("Testing", "Icons.BedFill", "RandomInfo"));
        halcyonNode.SubNodes.Add(_nodeFactory.CreateOpenEditorNode("Tab Test", "Icons.AirplaneFill", "SpritePage"));

        halcyonNode.SubNodes.Add(_nodeFactory.CreateOpenEditorNode("Constants", "Icons.ListFill"));

        var monstersRoot = _nodeFactory.CreateDataRootNode("Monsters", "Monsters", "Monsters", "Icons.GhostFill");


        //             new OpenEditorNode("Dev Control", "Icons.GameControllerFill", "DevControl"),
        //             new OpenEditorNode("Zone Editor", "Icons.StairsFill", "ZoneEditor"),
        //             new OpenEditorNode("Ground Editor", "Icons.MapTrifoldFill", "GroundEditor"),
        //             new OpenEditorNode("Testing", "Icons.BedFill", "RandomInfo"),


        monstersRoot.SubNodes.Add(_nodeFactory.CreateDataItemNode("eevee", "MonsterEditor", "eevee: Eevee",
            "Icons.GhostFill"));
        monstersRoot.SubNodes.Add(_nodeFactory.CreateDataItemNode("seviper", "MonsterEditor", "seviper: Seviper",
            "Icons.GhostFill"));

        var particlesRoot = _nodeFactory.CreateSpriteRootNode("particles", "", "Particles", "Icons.PaintBrushFill");
        particlesRoot.SubNodes.Add(_nodeFactory.CreateDataItemNode("Acid_Blue", "SpriteEditor", "Acid_Blue",
            "Icons.PaintBrushFill"));
        particlesRoot.SubNodes.Add(_nodeFactory.CreateDataItemNode("Acid_Red", "SpriteEditor", "Acid_Red",
            "Icons.PaintBrushFill"));

        halcyonNode.SubNodes.Add(particlesRoot);
        //             new NodeBase("Sprites", "Icons.PaintBrushFill")
        //             {
        //                 SubNodes = new ObservableCollection<NodeBase>
        //                 {
        //                     new NodeBase("Char Sprites", "Icons.GhostFill"),
        //                     new NodeBase("Portraits", "Icons.ImagesSquareFill"),
        //                     new ActionDataNode("Particles", "Icons.ShootingStarFill")
        //                     {
        //                         SubNodes = new ObservableCollection<NodeBase>
        //                         {
        //                             new NodeBase("Absorb", "Icons.ShootingStarFill"),
        //                             new NodeBase("Acid_Blue", "Icons.ShootingStarFill"),
        //                         }
        //                     },
        //                     new ActionDataNode("Beam", "Icons.HeadlightsFill")
        //                     {
        //                         SubNodes = new ObservableCollection<NodeBase>
        //                         {
        //                             new NodeBase("Beam_2", "Icons.HeadlightsFill"),
        //                             new NodeBase("Beam_Pink", "Icons.HeadlightsFill"),
        //                         }
        //                     },
        //                 }
        //             },
        //             new NodeBase("Mods", "Icons.SwordFill")
        //             {
        //                 SubNodes = new ObservableCollection<NodeBase>
        //                 {
        //                     new NodeBase("halcyon: Halcyon", "Icons.SwordFill"),
        //                     new NodeBase("zorea_mystery_dungeon: Zorea Mystery Dungeon", "Icons.SwordFill"),
        //                 }
        //             }
        //         }

        halcyonNode.SubNodes.Add(monstersRoot);
        Nodes = new ObservableCollection<NodeBase> { halcyonNode };

        halcyonNode.IsExpanded = true;
        monstersRoot.IsExpanded = true;
    }

    private void ApplyFilter(string filter)
    {
        foreach (var node in Nodes)
        {
            NodeHelper.FilterRecursive(node, filter);
        }
    }

    private TabSwitcherViewModel? _tabSwitcher;

    public TabSwitcherViewModel? TabSwitcher
    {
        get => _tabSwitcher;
        set => this.RaiseAndSetIfChanged(ref _tabSwitcher, value);
    }


    private ModSwitcherViewModel? _modSwitcher = null;

    public ModSwitcherViewModel? ModSwitcher
    {
        get => _modSwitcher;
        set => this.RaiseAndSetIfChanged(ref _modSwitcher, value);
    }
    

    // Navigate to a page from tab switcher
    public void NavigateToPage(PageNode node)
    {
        Console.WriteLine($"Navigating to page {node.Title}");
        Console.WriteLine("yay");
        ActivePage = node.Page;
    }

    public TreeSearchViewModel TreeSearch { get; } = new TreeSearchViewModel();
}