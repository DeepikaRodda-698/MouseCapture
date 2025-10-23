using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MouseAction = MouseCapture.Models.MouseAction;

namespace MouseCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<MouseAction> MouseActions { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MouseActions = new ObservableCollection<MouseAction>();
            StopButton.IsEnabled = false;
            DataContext = this;

            // Apply initial sorting
            ApplySorting();

        }

        private void ApplySorting()
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(MouseActions);
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new SortDescription(nameof(MouseAction.ActionOn), ListSortDirection.Descending));
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            MouseHook.StartMouseCapture();
            ((Button)sender).IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            MouseHook.StopMouseCapture();
            ((Button)sender).IsEnabled = false;
            StartButton.IsEnabled = true;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}