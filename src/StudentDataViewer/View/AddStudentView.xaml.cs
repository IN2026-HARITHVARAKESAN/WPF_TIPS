using System.Windows;
using System.Windows.Input;
using StudentDataViewer.ViewModel;

namespace StudentDataViewer.Views
{
    /// <summary>
    /// Interaction logic for AddStudentView.xaml
    /// </summary>
    public partial class AddStudentView : Window
    {
        private MainViewModel _mainViewModel;

        /// <summary>
        /// Sets dataContext and mainViewModel
        /// </summary>
        /// <param name="mainViewModel">The mainViewModel which has the data</param>
        public AddStudentView(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
            _mainViewModel = mainViewModel;
            SetStudentId();
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

        private void SetStudentId()
        {
            StudentId.Text = GenerateStudentId();
        }

        private string GenerateStudentId()
        {
            if(_mainViewModel.Students.Count == 0)
            {
                return "SOL001";
            }
            var lastStudentFullId = _mainViewModel.Students.Last().StudentId.ToString();
            var lastStudentRollNumber = lastStudentFullId.Substring(lastStudentFullId.Length - 3);
            var nextRollNumber = (int.Parse(lastStudentRollNumber) + 1).ToString(new string('0', lastStudentRollNumber.Length));
            return "SOL" + nextRollNumber;
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                string studentId = StudentId.Text;
                string name = Name.Text;
                string department = Department.Text;
                string section = Section.Text.Length > 0 ? Section.Text[0].ToString() : string.Empty;
                int year;
                double cgpa;

                bool isYearValid = int.TryParse(Year.Text, out year);
                bool isCgpaValid = double.TryParse(CGPA.Text, out cgpa);

                if (string.IsNullOrWhiteSpace(studentId) ||
                    string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(department) ||
                    section == string.Empty ||
                    !isYearValid ||
                    !isCgpaValid)
                {
                    MessageBox.Show("Please enter valid values for all fields.");
                    return;
                }

                var student = new Models.Student
                {
                    StudentId = studentId,
                    Name = name,
                    Department = department,
                    Section = section,
                    Year = year,
                    Cgpa = cgpa
                };

                _mainViewModel.Students.Add(student);
                MessageBox.Show($"New student {Name.Text} added to the list");
                _mainViewModel.Validation = new AddStudentViewModel();
                _mainViewModel.AddNewSection();
                this.Close();
            }
        }
    }
}
