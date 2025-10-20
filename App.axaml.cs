using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<DevControlViewModel>();
        collection.AddTransient<ZoneEditorPageViewModel>();
        collection.AddTransient<GroundEditorPageViewModel>();
        collection.AddTransient<RandomInfoPageViewModel>();
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
        
        
        var factory = services.GetRequiredService<PageFactory>();
        
        factory.Register<DevControlViewModel>("DevControl");
        factory.Register<ZoneEditorPageViewModel>("ZoneEditor");
        factory.Register<GroundEditorPageViewModel>("GroundEditor");
        factory.Register<RandomInfoPageViewModel>("RandomInfo");

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

        // TopLevel provider
        collection.AddSingleton<Func<TopLevel?>>(x => () =>
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime topWindow)
                return TopLevel.GetTopLevel(topWindow.MainWindow);
            
            if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                return TopLevel.GetTopLevel(singleViewPlatform.MainView);

            return null;
        });
        base.OnFrameworkInitializationCompleted();
    }
}