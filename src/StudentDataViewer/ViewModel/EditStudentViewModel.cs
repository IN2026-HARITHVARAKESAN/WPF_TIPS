using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using StudentDataViewer.Models;

namespace StudentDataViewer.ViewModel
{
    /// <summary>
    /// ViewModel responsible for editing a Student object. 
    /// Handles validation, property binding, and update logic.
    /// </summary>
    public class EditStudentViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Private Fields

        private Student _student;
        private string _name;
        private string _cgpa;
        private string _mark;
        private readonly MainViewModel _mainViewModel;
        private readonly Dictionary<string, string> Errors = new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Command to cancel the edit operation and close the window.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Command to save the edited student and close the window.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Event raised when a close request is triggered.
        /// </summary>
        public event Action<bool>? CloseRequested;

        /// <summary>
        /// Event raised when a save operation is requested.
        /// </summary>
        public event Action<bool>? SaveRequested;

        /// <summary>
        /// Gets or sets the current student being edited.
        /// </summary>
        public Student Student
        {
            get => _student;
            set
            {
                _student = value;
                Name = _student.Name;
                Cgpa = _student.Cgpa.ToString();
                Mark = _student.Mark.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the name of the student.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Student.Name = _name;
                ValidateName();
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(NameError));
            }
        }

        /// <summary>
        /// Gets or sets the CGPA of the student as string.
        /// </summary>
        public string Cgpa
        {
            get => _cgpa;
            set
            {
                _cgpa = value;
                if (double.TryParse(_cgpa, out double val))
                    Student.Cgpa = val;

                ValidateCgpa();
                OnPropertyChanged(nameof(Cgpa));
                OnPropertyChanged(nameof(CgpaError));
            }
        }

        /// <summary>
        /// Gets or sets the Mark of the student as string.
        /// </summary>
        public string Mark
        {
            get => _mark;
            set
            {
                _mark = value;
                if (int.TryParse(_mark, out int m))
                    Student.Mark = m;

                ValidateMark();
                OnPropertyChanged(nameof(Mark));
                OnPropertyChanged(nameof(MarkError));
            }
        }

        #region Validation

        /// <summary>
        /// Indicates whether the ViewModel currently has validation errors.
        /// </summary>
        public bool HasErrors => Errors.Count > 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && Errors.ContainsKey(propertyName))
                return new List<string> { Errors[propertyName] };

            return null;
        }

        /// <summary>
        /// Returns validation error message for the Name property if present.
        /// </summary>
        public string? NameError => Errors.ContainsKey(nameof(Name)) ? Errors[nameof(Name)] : null;

        /// <summary>
        /// Returns validation error message for the Cgpa property if present.
        /// </summary>
        public string? CgpaError => Errors.ContainsKey(nameof(Cgpa)) ? Errors[nameof(Cgpa)] : null;

        /// <summary>
        /// Returns validation error message for the Mark property if present.
        /// </summary>
        public string? MarkError => Errors.ContainsKey(nameof(Mark)) ? Errors[nameof(Mark)] : null;

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="EditStudentViewModel"/> class.
        /// </summary>
        /// <param name="student">The student being edited.</param>
        /// <param name="mainViewModel">The parent MainViewModel containing the student collection.</param>
        public EditStudentViewModel(Student student, MainViewModel mainViewModel)
        {
            Student = student;
            _mainViewModel = mainViewModel;
            CancelCommand = new RelayCommand(_ => OnCancel());
            SaveCommand = new RelayCommand(_ => OnSave());
        }

        #endregion

        #region Private Methods

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AddError(string propertyName, string error)
        {
            if (!Errors.ContainsKey(propertyName))
                Errors[propertyName] = error;
        }

        private void ClearError(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ValidateName()
        {
            ClearError(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name), "Name cannot be empty");
            else if (Name.Length < 3)
                AddError(nameof(Name), "Name must be at least 3 characters long");
        }

        private void ValidateCgpa()
        {
            ClearError(nameof(Cgpa));
            if (!double.TryParse(Cgpa, out double value))
                AddError(nameof(Cgpa), "Enter valid CGPA");
            else if (value < 0 || value > 10)
                AddError(nameof(Cgpa), "CGPA should be between 0 and 10");
        }

        private void ValidateMark()
        {
            ClearError(nameof(Mark));
            if (!int.TryParse(Mark, out int v))
                AddError(nameof(Mark), "Enter valid Mark");
            else if (v < 0 || v > 100)
                AddError(nameof(Mark), "Mark should be between 0 and 100");
        }

        private void OnCancel()
        {
            CloseRequested?.Invoke(true);
        }

        private void OnSave()
        {
            foreach (var studentRecord in _mainViewModel.Students)
            {
                if (studentRecord.StudentId == Student.StudentId)
                {
                    studentRecord.Name = Student.Name;
                    studentRecord.Department = Student.Department;
                    studentRecord.Section = Student.Section;
                    studentRecord.Year = Student.Year;
                    studentRecord.Cgpa = Student.Cgpa;
                    studentRecord.Mark = Student.Mark;
                }
            }
            // trigger chart refresh
            _mainViewModel.RefreshMarksPlot();
            CloseRequested?.Invoke(true);
        }

        #endregion
    }
}
