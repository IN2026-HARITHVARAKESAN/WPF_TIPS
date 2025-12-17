using System.Collections.ObjectModel;
using System.Windows;
using StudentDataViewer.Models;
using StudentDataViewer.ViewModel;

namespace StudentDataViewer.View
{
    /// <summary>
    /// Interaction logic for DeleteConfirmationView.xaml
    /// </summary>
    public partial class DeleteConfirmationView : Window
    {
        public DeleteConfirmationView(ObservableCollection<Student> students)
        {
            InitializeComponent();
            var viewModel = new DeleteConfirmationViewModel(students) ;
            DataContext = viewModel ;

            viewModel.CloseRequested += _ =>
            {
                this.Close();
            };
        }
    }
}
