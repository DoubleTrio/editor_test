using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaTest.ViewModels;
using ReactiveUI;

namespace AvaloniaTest.Services;


using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

public interface IDialogService
{
    Task<TResult?> ShowDialogAsync<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : class;
}

public class DialogService : IDialogService
{
    private readonly ViewLocator _viewLocator;

    public DialogService()
    {
        _viewLocator = new ViewLocator();
    }
    public DialogService(ViewLocator viewLocator)
    {
        _viewLocator = viewLocator;
    }

    public async Task<TResult?> ShowDialogAsync<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : class
    {
        var control = _viewLocator.Build(viewModel);
        if (control is not Window window)
            throw new InvalidOperationException($"View for {typeof(TViewModel).Name} must be a Window.");

        window.DataContext = viewModel;

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null)
            throw new InvalidOperationException("Main window not found for dialog parent.");
        
        return await window.ShowDialog<TResult?>(mainWindow);
    }
}

public class DialogService2(Func<TopLevel?> topLevel)
{
    public async Task ShowDialog<THost, TDialogViewModel>(THost host, TDialogViewModel dialogViewModel)
        where TDialogViewModel : DialogViewModel
        where THost : IDialogProvider
    {
        host.Dialog = dialogViewModel;
        dialogViewModel.Show();


        await dialogViewModel.WaitAsnyc();
    }
    
    public async Task ShowDialog<TDialogViewModel>(TDialogViewModel dialogViewModel)
        where TDialogViewModel : DialogViewModel
    {

        dialogViewModel.Show();
        
        await dialogViewModel.WaitAsnyc();
    }

    public async Task<string?> FolderPicker()
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return null;
        
        var folders = await topLevelVisual.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select a folder"
        });

        var path = folders.FirstOrDefault()?.Path;
        if (path == null) return null;
        return path.IsAbsoluteUri ? path.LocalPath : path.OriginalString;
    }
    
    public async Task<string[]> FilePicker(string title = "Select a file", bool allowMultiple = false, FilePickerFileType[]? fileTypes = null)
    {
        fileTypes ??= [FilePickerFileTypes.All];
        
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return [];
        
        var files = await topLevelVisual.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = allowMultiple,
            Title = title,
            FileTypeFilter = fileTypes
        });

        return files.Select(file => file.Path.IsAbsoluteUri ? file.Path.LocalPath : file.Path.OriginalString).ToArray();
    }
}