using MouseCapture.ViewModels;
using System.Windows;

namespace MouseCapture.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            if (DataContext is MainViewModel vm) vm.Dispose();
            base.OnClosed(e);
        }
    }
}