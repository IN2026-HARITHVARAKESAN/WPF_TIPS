using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using StudentDataViewer.Models;

namespace StudentDataViewer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private ObservableCollection<Student> _students;
        private ObservableCollection<Student> _studentsToDisplay;
        private ObservableCollection<string> _sections;
        private string _newSection;
        private Student _selectedStudent;
        private string _selectedSection;
        private PlotModel _marksPlot;

        #endregion

        #region Public Properties

        /// <summary>
        /// Validation view model to validate entered data in add student view
        /// </summary>
        public AddStudentViewModel Validation { get; set; }

        /// <summary>
        /// Holds the collection of students
        /// </summary>
        public ObservableCollection<Student> Students
        {
            get => _students;
            set
            {
                _students = value;
                OnPropertyChanged(nameof(Students));
            }
        }

        /// <summary>
        /// Collection of students to display in the data grid
        /// </summary>
        public ObservableCollection<Student> StudentsToDisplay
        {
            get => _studentsToDisplay;
            set
            {
                _studentsToDisplay = value;
                OnPropertyChanged(nameof(StudentsToDisplay));
            }
        }

        /// <summary>
        /// Collection of sections
        /// </summary>
        public ObservableCollection<string> Sections
        {
            get => _sections;
            set
            {
                _sections = value;
                OnPropertyChanged(nameof(Sections));
            }
        }

        /// <summary>
        /// Sections collection with "ALL" prepended for selection purposes
        /// </summary>
        public IEnumerable<string> SectionsWithAll
        {
            get
            {
                yield return "ALL";
                foreach (var s in Sections)
                    yield return s;
            }
        }

        /// <summary>
        /// It will update if User entered new section.
        /// </summary>
        public string NewSection
        {
            get => _newSection;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _newSection = value[0].ToString();
                else
                    _newSection = string.Empty;

                OnPropertyChanged(nameof(NewSection));
            }
        }

        /// <summary>
        /// Holds user selected student from the table
        /// </summary>
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        /// <summary>
        /// Hold current selected section for filtering.
        /// </summary>
        public string SelectedSection
        {
            get => _selectedSection;
            set
            {
                if (_selectedSection != value)
                {
                    _selectedSection = value;
                    SetStudentsToDisplay();
                    OnPropertyChanged(nameof(SelectedSection));
                }
            }
        }

        /// <summary>
        /// Plot model for displaying marks
        /// </summary>
        public PlotModel MarksPlot
        {
            get => _marksPlot;
            private set
            {
                _marksPlot = value;
                OnPropertyChanged(nameof(MarksPlot));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates object for Students collection and generate test data.
        /// </summary>
        public MainViewModel()
        {
            Students = new ObservableCollection<Student>();
            StudentsToDisplay = new ObservableCollection<Student>();
            Sections = new ObservableCollection<string>();
            Validation = new AddStudentViewModel();
            GetTestDataForStudents();

            Sections.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SectionsWithAll));
            };

            Students.CollectionChanged += (s, e) =>
            {
                SetStudentsToDisplay();
                RefreshMarksPlot();
            };

            BuildMarksPlot();
        }

        /// <summary>
        /// Add a student in the collection.
        /// </summary>
        /// <param name="student">Student object to add</param>
        public void AddStudent(Student student)
        {
            Students.Add(student);
            RefreshMarksPlot();
        }

        /// <summary>
        /// Add a new section to the Sections collection if it does not exist
        /// </summary>
        public void AddNewSection()
        {
            string input = NewSection;
            if (!string.IsNullOrWhiteSpace(input) && !Sections.Contains(input))
            {
                Sections.Add(input);
            }
        }

        /// <summary>
        /// Refreshes the marks plot based on the current data.
        /// </summary>
        public void RefreshMarksPlot()
        {
            var data = StudentsToDisplay?.ToList() ?? Students.ToList();
            var pm = new PlotModel { Title = "Marks (ID vs Mark)" };

            // Horizontal bar chart: Category on Y, values on X
            pm.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = data,
                LabelField = nameof(Student.StudentId),
                IsZoomEnabled = true,
                IsPanEnabled = true
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 100,
                Title = "Mark",
                MajorStep = 10
            });

            var series = new BarSeries
            {
                ItemsSource = data.Select(s => new BarItem(s.Mark)),
                FillColor = OxyColors.ForestGreen,
                LabelFormatString = "{0}"
            };

            pm.Series.Add(series);
            MarksPlot = pm;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Private Methods

        private void SetStudentsToDisplay()
        {
            StudentsToDisplay.Clear();
            if (SelectedSection == "ALL")
            {
                foreach (var student in Students)
                {
                    StudentsToDisplay.Add(student);
                }
            }
            else
            {
                foreach (var student in Students)
                {
                    if (student != null && student.Section == SelectedSection)
                    {
                        StudentsToDisplay.Add(student);
                    }
                }
            }
            RefreshMarksPlot();
        }

        private void GetTestDataForStudents()
        {
            Students.Add(new Student
            {
                IsSelected = false,
                StudentId = "S001",
                Name = "Arun Kumar",
                Department = "CSE",
                Section = "A",
                Year = 3,
                Cgpa = 8.6,
                Mark = 85
            });
            Students.Add(new Student
            {
                IsSelected = false,
                StudentId = "S002",
                Name = "Rahul Mehta",
                Department = "MECH",
                Section = "B",
                Year = 4,
                Cgpa = 8.2,
                Mark = 78
            });
            Students.Add(new Student
            {
                IsSelected = false,
                StudentId = "S003",
                Name = "Priya Sharma",
                Department = "CIVIL",
                Section = "B",
                Year = 2,
                Cgpa = 7.9,
                Mark = 72
            });
        }

        private void BuildMarksPlot() => RefreshMarksPlot();

        #endregion
    }
}
