using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaTest.Views
{
    public partial class RenameWindow : Window
    {
        public RenameWindow()
        {
        }

        public void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close(true);
        }


        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(false);
        }
    }
}
