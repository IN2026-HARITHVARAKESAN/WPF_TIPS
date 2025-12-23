using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

public class EditStudentPageTests : IDisposable
{
    private readonly WindowsDriver<WindowsElement> _session;
    private readonly MainWindowPage _mainWindow;
    private readonly EditStudentPage _editStudent;

    public EditStudentPageTests()
    {
        var appCapabilities = new AppiumOptions();
        appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
        appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
        _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
        _mainWindow = new MainWindowPage(_session);
        _editStudent = new EditStudentPage(_session);
    }

    [Fact]
    [Trait("TestCaseId", "75849")]
    public void EditStudent_FullWorkflow_Test()
    {
        // 1. Main window should open with Student's data grid and 3 buttons
        Assert.NotNull(_mainWindow.StudentDataGrid);
        Assert.NotNull(_mainWindow.AddNewStudentButton);
        Assert.NotNull(_mainWindow.EditStudentButton);
        Assert.NotNull(_mainWindow.DeleteStudentButton);

        // 2. Click Edit Student Button (no selection)
        _mainWindow.EditStudent();
        Assert.Throws<OpenQA.Selenium.WebDriverException>(() =>
            _editStudent.NameTextBox.Displayed);

        // 3. Select section "All" in Student Data List drop down
        _mainWindow.SectionsComboBox.Click();
        _mainWindow.SectionsComboBox.SendKeys("ALL");
        _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);
        Assert.Equal("ALL", _mainWindow.SectionsComboBox.Text);
        Assert.True(_mainWindow.StudentDataGrid.Displayed);

        // 4. Click on any student details (first row)
        var firstRow = _mainWindow.StudentDataGrid.FindElementByXPath("//DataItem[1]");
        firstRow.Click();
        Assert.True(firstRow.Selected);

        // 5. Click Edit Student Button (with selection)
        _mainWindow.EditStudent();
        Assert.True(_editStudent.NameTextBox.Displayed);
        Assert.True(_editStudent.DepartmentComboBox.Displayed);
        Assert.True(_editStudent.SectionComboBox.Displayed);
        Assert.True(_editStudent.YearComboBox.Displayed);
        Assert.True(_editStudent.CGPATextBox.Displayed);
        Assert.True(_editStudent.SaveButton.Displayed);
        Assert.True(_editStudent.CloseButton.Displayed);

        // 6. Change the name of the student in the Name text box
        string newName = "Harith";
        _editStudent.NameTextBox.Clear();
        _editStudent.NameTextBox.SendKeys(newName);
        Assert.Equal(newName, _editStudent.NameTextBox.Text);

        // 7. Click Save Button
        _editStudent.Save();
        Assert.Throws<OpenQA.Selenium.WebDriverException>(() =>
            _editStudent.NameTextBox.Displayed);

        // Verify updated name in grid
        var firstRowName = _mainWindow.StudentDataGrid.FindElementByXPath("//DataItem[1]/Custom[3]");
        Assert.Equal("Harith", firstRowName.Text);

        // 8. Click Edit Student Button again
        _mainWindow.EditStudent();
        Assert.True(_editStudent.NameTextBox.Displayed);

        // 9. Click Cancel Button
        _editStudent.Cancel();
        Assert.Throws<OpenQA.Selenium.WebDriverException>(() =>
            _editStudent.NameTextBox.Displayed);
    }

    public void Dispose()
    {
        _session?.Quit();
    }
}