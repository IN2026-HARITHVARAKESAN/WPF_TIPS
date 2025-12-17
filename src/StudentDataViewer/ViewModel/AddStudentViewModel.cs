using System.Collections;
using System.ComponentModel;

namespace StudentDataViewer.ViewModel
{
    public class AddStudentViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Private Fields

        private string _name;
        private string _cgpa;
        private readonly Dictionary<string, string> Errors = new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Name of the student.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                ValidateName();
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(NameError));
            }
        }

        /// <summary>
        /// Gets or sets the CGPA of the student.
        /// </summary>
        public string Cgpa
        {
            get { return _cgpa; }
            set
            {
                _cgpa = value;
                ValidateCgpa();
                OnPropertyChanged(nameof(Cgpa));
                OnPropertyChanged(nameof(CgpaError));
            }
        }

        /// <summary>
        /// Indicates whether the view model currently has validation errors.
        /// </summary>
        public bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// Holds the error message of name field.
        /// </summary>
        public string? NameError => Errors.ContainsKey(nameof(Name)) ? Errors[nameof(Name)] : null;

        /// <summary>
        /// Holds the error message of CGPA field.
        /// </summary>
        public string? CgpaError => Errors.ContainsKey(nameof(Cgpa)) ? Errors[nameof(Cgpa)] : null;

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// If the property has error, it will return the error.
        /// </summary>
        /// <param name="propertyName"> property name [e.g: Name, Cgpa] </param>
        /// <returns>The error of the property </returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (Errors.ContainsKey(propertyName))
                return Errors[propertyName];
            return null;
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
            Errors.Remove(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidateName()
        {
            ClearError(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name), "Name cannot be empty");
            else if (Name.Length < 3)
                AddError(nameof(Name), "Name must be at least 3 characters long");
            else
            {
                char[] invalidChars = { '?', '>', '<', '%', '!', '$', '^', '&', '*' };

                foreach (char c in invalidChars)
                {
                    if (Name.Contains(c))
                    {
                        AddError(nameof(Name), "Name should not contain '?><!$%^&*' characters");
                        break;
                    }
                }
            }
        }

        private void ValidateCgpa()
        {
            ClearError(nameof(Cgpa));
            if (!double.TryParse(Cgpa, out double value))
            {
                AddError(nameof(Cgpa), "Enter valid CGPA");
            }
            else if (value < 0 || value > 10)
            {
                AddError(nameof(Cgpa), "CGPA should be between 0 and 10");
            }
        }

        #endregion
    }
}
