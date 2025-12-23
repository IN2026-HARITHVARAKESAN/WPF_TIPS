using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;
using System.Threading;

public class AddStudentPageTests : IDisposable
{
    private readonly WindowsDriver<WindowsElement> _session;
    private readonly MainWindowPage _mainWindow;
    private readonly AddStudentPage _addStudent;

    public AddStudentPageTests()
    {
        var appCapabilities = new AppiumOptions();
        appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
        appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
        _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
        _mainWindow = new MainWindowPage(_session);
        _addStudent = new AddStudentPage(_session);
    }

    [Fact]
    public void AddStudent_FullWorkflow_Test()
    {
        // 1. Main window should open with Student's data grid and 3 buttons
        Assert.NotNull(_mainWindow.StudentDataGrid);
        Assert.NotNull(_mainWindow.AddNewStudentButton);
        Assert.NotNull(_mainWindow.EditStudentButton);
        Assert.NotNull(_mainWindow.DeleteStudentButton);

        // 2. Click Add New Student Button
        _mainWindow.AddNewStudent();
        Assert.True(_addStudent.NameTextBox.Displayed);
        Assert.True(_addStudent.AddStudentButton.Displayed);
        Assert.True(_addStudent.CancelButton.Displayed);

        // 3. Click Cancel Button
        _addStudent.Cancel();
        Thread.Sleep(500); // Wait for window to close
        Assert.Throws<OpenQA.Selenium.WebDriverException>(() => _addStudent.NameTextBox.Displayed);

        // 4. Click Add New Student Button again
        _mainWindow.AddNewStudent();
        Assert.True(_addStudent.NameTextBox.Displayed);

        // 5. Enter Name
        _addStudent.NameTextBox.Clear();
        _addStudent.NameTextBox.SendKeys("Harith");
        Assert.Equal("Harith", _addStudent.NameTextBox.Text);

        // 6. Choose Department "IT"
        _addStudent.DepartmentComboBox.Click();
        _addStudent.DepartmentComboBox.SendKeys("IT");
        _addStudent.DepartmentComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);

        // 7. Enter Section "B"
        _addStudent.SectionComboBox.Click();
        _addStudent.SectionComboBox.Clear();
        _addStudent.SectionComboBox.SendKeys("B");
        Assert.Equal("B", _addStudent.SectionComboBox.Text);

        // 8. Choose Year "2"
        _addStudent.YearComboBox.Click();
        _addStudent.YearComboBox.SendKeys("2");
        _addStudent.YearComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);

        // 9. Enter CGPA (e.g., 8.5)
        _addStudent.CGPATextBox.Clear();
        _addStudent.CGPATextBox.SendKeys("8.5");
        Assert.Equal("8.5", _addStudent.CGPATextBox.Text);

        // 10. Click Add Student Button
        _addStudent.AddStudent();
        Thread.Sleep(500); // Wait for message box to appear

        // 11. Handle Message Box (assume standard Windows message box)
        var popupWindow = _session.WindowHandles.Last();
        _session.SwitchTo().Window(popupWindow);
        var messageBox = _session.FindElementByName("OK");
        Assert.NotNull(messageBox);
        messageBox.Click();
        Thread.Sleep(500); // Wait for popup and add window to close

        // 12. Select section "B" in Students data list
        _session.SwitchTo().Window(_session.WindowHandles.First());
        _mainWindow.SectionsComboBox.Click();
        _mainWindow.SectionsComboBox.SendKeys("B");
        _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);
        Thread.Sleep(500); // Wait for grid to update

        // Verify the new student is in the grid
        var studentRows = _mainWindow.StudentDataGrid.FindElementsByXPath("//DataItem/Custom[3]");
        bool found = false;
        foreach (var row in studentRows)
        {
            if (row.Text.Contains("Harith"))
            {
                found = true;
                break;
            }
        }
        Assert.True(found, "Newly added student 'Harith' should be present in the grid.");
    }

    public void Dispose()
    {
        _session?.Quit();
    }
}