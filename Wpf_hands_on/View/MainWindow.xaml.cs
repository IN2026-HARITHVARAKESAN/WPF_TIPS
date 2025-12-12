using System.Windows;
using Wpf_hands_on.Models;

namespace Wpf_hands_on
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Employee> employees { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            employees = new List<Employee>();
            employees.Add(new Employee() { Name = "Hari", JobTitle = "PE", MailId = "Hari@gmail.com", PhoneNumber = "1234567890" });
            employees.Add(new Employee() { Name = "Harith", JobTitle = "PE", MailId = "Hari@gmail.com", PhoneNumber = "1234567890" });
            this.DataContext = employees;
            EmployeeListGrid.ItemsSource = employees;
            EmployeeListGrid.SelectedItem = employees[0];
        }

        private void EmployeeListGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Employee employee = (Employee)EmployeeListGrid.SelectedItem;

            EmpName.Text =  ": " + employee.Name;
            EmpJobTitle.Text = ": " + employee.JobTitle;
            EmpEmail.Text = ": " + employee.MailId;
            EmpPhone.Text = ": " + employee.PhoneNumber;
        }
    }
}