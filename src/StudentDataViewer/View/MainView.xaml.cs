using System.Windows;
using System.Windows.Input;
using StudentDataViewer.ViewModel;

namespace StudentDataViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private MainViewModel viewModel;

        /// <summary>
        /// Sets DataContext and ViewModel.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
