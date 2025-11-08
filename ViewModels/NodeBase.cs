using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaTest.Services;
using AvaloniaTest.Views;
using ReactiveUI;

namespace AvaloniaTest.ViewModels;

using System.Collections.ObjectModel;

public class NodeBase : ViewModelBase
{
    protected readonly IDialogService _dialogService;
    protected readonly NodeFactory _nodeFactory;
    public ObservableCollection<NodeBase> SubNodes { get; set; }

    // public NodeBase? Parent { get; internal set; } 
    
    private string _title = "";

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    public string Icon { get; }

    public NodeBase(IDialogService dialogService, string title = "", string? icon = null)
    {
        _dialogService = dialogService;
        Title = title;
        Icon = icon ?? "";
        SubNodes = new ObservableCollection<NodeBase>();
        IsExpanded = false;
        // SubNodes.CollectionChanged += OnSubNodesChanged;
    }
    
    // public NodeBase(string title, string icon, ObservableCollection<NodeBase> subNodes)
    // {
    //     Title = title;
    //     Icon = icon;
    //     SubNodes = subNodes;
    //     IsExpanded = false;
    //     SubNodes.CollectionChanged += OnSubNodesChanged;
    // }
    
    
    private bool _isExpanded = false;
    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }
    
    private bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }
    
    // private void OnSubNodesChanged(object? s, NotifyCollectionChangedEventArgs e) {
    //     if (e.NewItems != null)
    //         foreach(NodeBase child in e.NewItems) child.Parent = this;
    //     if (e.OldItems != null)
    //         foreach(NodeBase child in e.OldItems) child.Parent = null;
    // }


}

public class OpenEditorNode : NodeBase
{
    // public string EditorTabTitle { get; }

    // public ReactiveCommand<Unit, Unit> OpenEditorCommand { get; }

    public string EditorKey { get; }
    
    public OpenEditorNode(IDialogService dialogService, string title, string? icon = null, string editorKey = "") 
        : base(dialogService, title, icon ?? "")
    {
        EditorKey = editorKey;
    }
}

public class ActionDataNode : NodeBase
{
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public ActionDataNode(IDialogService dialogService, string title, string? icon = null) 
        : base(dialogService, title, icon ?? "")
    {
        AddCommand = ReactiveCommand.Create(() => Console.WriteLine("Add command yayk"));
        DeleteCommand = ReactiveCommand.Create(() => Console.WriteLine("Delete command"));
    }
}

public class DataItemNode : OpenEditorNode
{
    // public ReactiveCommand<Unit, Unit> DataDeleteCommand { get; }

    public string ItemKey { get; }
    
    private string _value = "";
    public string Value 
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public DataItemNode(IDialogService dialogService, string itemKey, string editorKey, string? title, string? icon = null) 
        : base(dialogService,title ?? "", icon ?? "", editorKey)
    {
        ItemKey = itemKey;
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Console.WriteLine("deleting");
            // await
        });
        // this.WhenAnyValue(x => x.Value).Subscribe(v => Title = $"{ItemKey}: {v}");
        
        // DataDeleteCommand = ReactiveCommand.Create(() => Console.WriteLine("Delete command"));
    }
}


public class DataRootNode : OpenEditorNode
{
    // Change to enum later!
    private readonly string _dataType;
    private readonly string key;
    private NodeFactory _nodeFactory;
    
    
    // TODO: Prompt the user to add the page... for that datatype
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<DataItemNode, Unit> DeleteCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReIndexCommand { get; }
    public ReactiveCommand<Unit, Unit> ResaveAllAsFileCommand { get; }
    public ReactiveCommand<Unit, Unit> ResaveAllAsDiffCommand { get; }
    // public ReactiveCommand<DataItemNode, Unit> PatchCommand { get; }
    // public ReactiveCommand<DataItemNode, Unit> PatchCommand { get; }
    // public ReactiveCommand<string, Unit> PatchCommand { get; }
    
    
    public ReactiveCommand<DataItemNode, Unit> ResaveAsFile { get; }
    public ReactiveCommand<DataItemNode, Unit> ResaveAsPatch { get; }
    public DataRootNode(IDialogService dialogService, NodeFactory nodeFactory, string dataType, string editorKey, string title, string? icon = null) 
        : base(dialogService, title, icon ?? "", editorKey)
    {
        _dataType = dataType;
        _nodeFactory = nodeFactory;
        
        Console.WriteLine(_dialogService);

        DeleteCommand = ReactiveCommand.CreateFromTask<DataItemNode>(async (node) =>
        {
            await MessageBoxWindowView.Show("TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd TRjjdkkdkddkd", "how are you", MessageBoxWindowView.MessageBoxButtons.YesNo, _dialogService);
            // RenameWindowViewModel vm = new RenameWindowViewModel();
            // bool result = await _dialogService.ShowDialogAsync<RenameWindowViewModel, bool>(vm);

            // if (!result)
            //     return;
            
            SubNodes.Remove(node);
            
 
            
            Console.WriteLine($"Deleting {node.Title} of type {_dataType}");
        });
        
       
        ReIndexCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Console.WriteLine("Reindexing");
            // await
        });
        AddCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            RenameWindowViewModel vm = new RenameWindowViewModel();

            // await _dialogService.ShowDialogAsync<RenameWindowViewModel, bool>(vm);
            
            
            bool result = await _dialogService.ShowDialogAsync<RenameWindowViewModel, bool>(vm);

            if (!result)
                return;

         
            SubNodes.Add(_nodeFactory.CreateDataItemNode(vm.Name, "MonsterEditor", vm.Name + ":", "Icons.GhostFill"));
            
            // RenameWindowView window = new RenameWindowView();
            // RenameWindowViewModel vm = new RenameWindowViewModel();
            // window.DataContext = vm;
            // Window? mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            // bool result = await window.ShowDialog<bool>(mainWindow);
            // if (!result)
            // return;

            // Console.WriteLine(window.DataContext);

            // lock (GameBase.lockObj)
            // {
            //     string assetName = Text.GetNonConflictingName(Text.Sanitize(vm.Name).ToLower(), DataManager.Instance.DataIndices[dataType].ContainsKey);
            //     DataManager.Instance.ContentChanged(dataType, assetName, createOp());
            //     string newName = DataManager.Instance.DataIndices[dataType].Get(assetName).GetLocalString(true);
            //     choices.AddEntry(assetName, newName);
            //
            //     if (dataType == DataManager.DataType.Zone)
            //         LuaEngine.Instance.CreateZoneScriptDir(assetName);
            // }
            // var item = new DataItemNode("new key", "value");
            // SubNodes.Add(item);
            // var 
            // Console.WriteLine($"Now adding a {node.Title} of type {_dataType}");


            // DataManager.Remove(assetName, _dataType);
        });
    }
}


// Used by TabSwitcher
public class PageNode : NodeBase
{
    public EditorPageViewModel Page { get; }
    public PageNode Parent { get; set; }
    public bool IsTopLevel => Parent == null;

    public PageNode(IDialogService dialogService, EditorPageViewModel page, PageNode parent = null)
        : base(dialogService, page.Title, page.Icon)
    {
        Page = page;
        Parent = parent;

        // // Listen for title changes on the page
        // page.PropertyChanged += (s, e) => 
        // {
        //     if (e.PropertyName == nameof(EditorPageViewModel.Title))
        //         OnPropertyChanged(nameof(Title));
        // };
    }
    
    public new string Title => Page.Title;
    
    public PageNode AddChild(IDialogService dialogService, EditorPageViewModel childPage)
    {
        var childNode = new PageNode(dialogService, childPage, this);
        SubNodes.Add(childNode);
        return childNode;
    }
    
    public void RemoveChild(PageNode childNode)
    {
        SubNodes.Remove(childNode);
        childNode.Parent = null;
    }
}