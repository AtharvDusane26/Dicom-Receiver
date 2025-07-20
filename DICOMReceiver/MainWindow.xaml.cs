using ApplicationBase;
using ApplicationBase.View;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DICOMReceiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IApplicationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Close(IDynamicView dynamicView)
        {
            var window = Window.GetWindow((UIElement)dynamicView);
            window.Close();
        }

        public void Launch(IDynamicView dynamicView, WindowCreationArgs args = null)
        {
            var window = new Window();
            window.Owner = this;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Content = dynamicView as UIElement;
            window.Show();
        }

        public void LaunchAsDialog(IDynamicView dynamicView, DialogueBoxCreationArgs args = null)
        {
            var window = new Window();
            window.Owner = this;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Content = dynamicView as UIElement;
            window.ShowDialog();
        }

        public ApplicationBase.MessageBoxResult ShowMessageBox(string message, MessageBoxEventArgs args = null)
        {
           return (ApplicationBase.MessageBoxResult)MessageBox.Show(message, args.Title, (System.Windows.MessageBoxButton)args.Button, (System.Windows.MessageBoxImage)args.Image);
        }
    }
}