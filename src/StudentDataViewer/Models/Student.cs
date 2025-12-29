using System.ComponentModel;

namespace StudentDataViewer.Models
{
    /// <summary>
    /// Represents a student with details like ID, name, department, section, year, CGPA, and selection state.
    /// Implements <see cref="INotifyPropertyChanged"/> for UI binding support.
    /// </summary>
    public class Student : INotifyPropertyChanged
    {
        private string _studentId;
        private string _name;
        private string _department;
        private string _section;
        private int _year;
        private double _cgpa;
        private int _mark;
        private bool _isSelected;

        /// <summary>
        /// Gets or sets the unique student ID.
        /// </summary>
        public string StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged(nameof(StudentId));
            }
        }

        /// <summary>
        /// Gets or sets the full name of the student.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the department of the student (e.g., CSE, MECH, CIVIL).
        /// </summary>
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged(nameof(Department));
            }
        }

        /// <summary>
        /// Gets or sets the section of the student (e.g., A, B, C).
        /// </summary>
        public string Section
        {
            get => _section;
            set
            {
                _section = value;
                OnPropertyChanged(nameof(Section));
            }
        }

        /// <summary>
        /// Gets or sets the current year of study.
        /// </summary>
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        /// <summary>
        /// Gets or sets the CGPA (Cumulative Grade Point Average) of the student.
        /// </summary>
        public double Cgpa
        {
            get => _cgpa;
            set
            {
                _cgpa = value;
                OnPropertyChanged(nameof(Cgpa));
            }
        }

        /// <summary>
        /// Gets or sets the mark (0-100) of the student.
        /// </summary>
        public int Mark
        {
            get => _mark;
            set
            {
                _mark = value;
                OnPropertyChanged(nameof(Mark));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this student is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Helper method to notify that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
