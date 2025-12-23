using System.Windows;
using System.Windows.Input;
using StudentDataViewer.Models;
using StudentDataViewer.ViewModel;

namespace StudentDataViewer.View
{
    /// <summary>
    /// Interaction logic for EditStudentView.xaml
    /// </summary>
    public partial class EditStudentView : Window
    {
        private EditStudentViewModel viewModel;

        /// <summary>
        /// Sets data context
        /// </summary>
        /// <param name="selectedStudent">User selected student</param>
        /// <param name="mainViewModel">main view model</param>
        public EditStudentView(Student selectedStudent, MainViewModel mainViewModel)
        {
            InitializeComponent();
            viewModel = new EditStudentViewModel(selectedStudent, mainViewModel);
            DataContext = viewModel;

            viewModel.CloseRequested += _ =>
            {
                this.Close();
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
