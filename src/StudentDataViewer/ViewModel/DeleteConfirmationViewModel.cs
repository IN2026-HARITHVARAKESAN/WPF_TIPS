using System.Collections.ObjectModel;
using System.Windows.Input;
using StudentDataViewer.Models;

namespace StudentDataViewer.ViewModel
{
    /// <summary>
    /// ViewModel for delete confirmation dialog. 
    /// Handles user confirmation and deletion of selected students.
    /// </summary>
    public class DeleteConfirmationViewModel
    {
        #region Private Fields

        private readonly ObservableCollection<Student> _students;

        #endregion

        #region Public Properties

        /// <summary>
        /// List of students selected for deletion.
        /// </summary>
        public List<Student> SelectedStudents { get; private set; } = new List<Student>();

        /// <summary>
        /// Command executed when user confirms deletion.
        /// </summary>
        public ICommand ConfirmCommand { get; }

        /// <summary>
        /// Command executed when user cancels deletion.
        /// </summary>
        public ICommand CancelCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the dialog should be closed.
        /// Parameter: true if close is requested.
        /// </summary>
        public event Action<bool>? CloseRequested;

        /// <summary>
        /// Raised when deletion is confirmed.
        /// Parameter: true if deletion succeeded.
        /// </summary>
        public event Action<bool>? DeleteConfirmed;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes DeleteConfirmationViewModel with student collection.
        /// </summary>
        /// <param name="students">Observable collection of all students.</param>
        public DeleteConfirmationViewModel(ObservableCollection<Student> students)
        {
            _students = students;
            SelectedStudents = students.Where(s => s.IsSelected).ToList();
            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        #endregion

        #region Private Methods

        private void ExecuteCancel(object? obj = null)
        {
            CloseRequested?.Invoke(true);
        }

        private void ExecuteConfirm(object? obj = null)
        {
            if (SelectedStudents.Count > 0)
            {
                foreach (var student in SelectedStudents.ToList())
                {
                    _students.Remove(student);
                }

                DeleteConfirmed?.Invoke(true);
            }
            else
            {
                DeleteConfirmed?.Invoke(false);
            }

            CloseRequested?.Invoke(true);
        }

        private bool CanExecuteConfirm(object? obj = null)
        {
            return SelectedStudents.Any();
        }

        #endregion
    }
}
