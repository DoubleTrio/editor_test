using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaTest.Services;
using AvaloniaTest.ViewModels;

namespace AvaloniaTest.Views
{
    public partial class MessageBoxWindowView : ChromelessWindow
    {
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No
        }

        public MessageBoxWindowView()
        {
            this.InitializeComponent();
        }

        public static async Task<MessageBoxResult> Show(
            string text,
            string title,
            MessageBoxButtons buttons,
            IDialogService dialogService)
        {
            var vm = new MessageBoxWindowViewModel(title, text);

            void AddButton(string caption, MessageBoxResult result)
            {
                vm.Buttons.Add(new MessageBoxWindowViewModel.ButtonViewModel(caption, result, vm));
            }

            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                    AddButton("Ok", MessageBoxResult.Ok);
                    break;
                case MessageBoxButtons.OkCancel:
                    AddButton("Ok", MessageBoxResult.Ok);
                    AddButton("Cancel", MessageBoxResult.Cancel);
                    break;
                case MessageBoxButtons.YesNo:
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No);
                    AddButton("Cancel", MessageBoxResult.Cancel);
                    break;
            }
            
            vm.CloseRequested += (_, result) => dialogService.Close(vm, result);

            var resultValue = await dialogService.ShowDialogAsync<MessageBoxWindowViewModel, MessageBoxResult>(vm, title);
            return resultValue;
        }
    }
}
