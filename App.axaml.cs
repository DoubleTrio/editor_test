using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaTest.Services;
using AvaloniaTest.ViewModels;
using AvaloniaTest.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaTest;


public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<TabEvents>();
        collection.AddSingleton<PageFactory>();
        collection.AddSingleton<NodeFactory>();
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<DevControlViewModel>();
        collection.AddTransient<ZoneEditorPageViewModel>();
        collection.AddTransient<GroundEditorPageViewModel>();
        collection.AddTransient<RandomInfoPageViewModel>();
         
        
        
        
        collection.AddTransient<NodeBase>();
        collection.AddTransient<ActionDataNode>();
        collection.AddTransient<DataRootNode>();
        collection.AddTransient<DataItemNode>();
        collection.AddTransient<OpenEditorNode>();
        collection.AddTransient<PageNode>();
        
        // TODO: remove?
        collection.AddSingleton<Func<Type, NodeBase>>(x => type => type switch
        {
            _ when type == typeof(NodeBase) => x.GetRequiredService<NodeBase>(),
            _ when type == typeof(ActionDataNode) => x.GetRequiredService<ActionDataNode>(),
            _ when type == typeof(DataRootNode) => x.GetRequiredService<DataRootNode>(),
            _ when type == typeof(DataItemNode) => x.GetRequiredService<DataItemNode>(),
            _ when type == typeof(OpenEditorNode) => x.GetRequiredService<OpenEditorNode>(),
            _ when type == typeof(PageNode) => x.GetRequiredService<PageNode>(),
            _ => throw new InvalidOperationException($"Page of type {type?.FullName} has no view model"),
        });
        
        collection.AddSingleton<ViewLocator>();
        collection.AddSingleton<IDialogService, DialogService>();
        
        
    }
    
    public static void RegisterPages(this IServiceProvider provider)
    {
        var pageFactory = provider.GetRequiredService<PageFactory>();

        pageFactory.Register<DevControlViewModel>("DevControl");
        pageFactory.Register<ZoneEditorPageViewModel>("ZoneEditor");
        pageFactory.Register<GroundEditorPageViewModel>("GroundEditor");
        pageFactory.Register<RandomInfoPageViewModel>("RandomInfo");
    }
}

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        var services = collection.BuildServiceProvider();
        
        services.RegisterPages();
        // TopLevel provider
        collection.AddSingleton<Func<TopLevel?>>(x => () =>
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow)
                return TopLevel.GetTopLevel(topWindow.MainWindow);
            
            if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                return TopLevel.GetTopLevel(singleViewPlatform.MainView);

            return null;
        });
        
        var vm = services.GetRequiredService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = vm
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}