using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium;
using System;
using System.Linq;
using System.Threading;

[TestFixture]
public class EditStudentPageTests
{
    private WindowsDriver<WindowsElement> _session;
    private MainWindowPage _mainWindow;
    private EditStudentPage _editStudentPage;

    [SetUp]
    public void Setup()
    {
        var appCapabilities = new AppiumOptions();
        appCapabilities.AddAdditionalCapability("app", @"Path\To\Your\StudentDataViewer.exe");
        appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
        _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);

        _mainWindow = new MainWindowPage(_session);
    }

    [TearDown]
    public void TearDown()
    {
        _session?.Quit();
    }

    [Test]
    public void EditStudentWorkflow()
    {
        // 1. Main window should open with data grid and 3 buttons
        Assert.IsTrue(_mainWindow.IsLoaded());
        Assert.IsTrue(_mainWindow.HasStudentDataGrid());
        Assert.IsTrue(_mainWindow.HasAddEditDeleteButtons());

        // 2. Click Edit Student Button (no selection)
        _mainWindow.ClickEditStudent();
        Assert.IsFalse(_mainWindow.IsEditStudentPopupOpen());

        // 3. Select section or "All"
        _mainWindow.SelectSection("All");
        Assert.IsTrue(_mainWindow.IsStudentListFiltered("All"));

        // 4. Click on any student details
        var student = _mainWindow.SelectFirstStudent();
        Assert.IsNotNull(student);

        // 5. Click Edit Student Button
        _mainWindow.ClickEditStudent();
        _editStudentPage = new EditStudentPage(_session);
        Assert.IsTrue(_mainWindow.IsEditStudentPopupOpen());
        Assert.AreEqual(student.Name, _editStudentPage.GetName());
        Assert.AreEqual(student.StudentId, _editStudentPage.GetStudentId());

        // 6. Change the name of the student
        var newName = student.Name + "_Edited";
        _editStudentPage.SetName(newName);

        // 7. Click Save Button
        _editStudentPage.Save();
        Assert.IsFalse(_mainWindow.IsEditStudentPopupOpen());
        Assert.IsTrue(_mainWindow.IsStudentNameUpdated(student.StudentId, newName));

        // 8. Click Edit Student Button again
        _mainWindow.SelectStudentById(student.StudentId);
        _mainWindow.ClickEditStudent();
        Assert.IsTrue(_mainWindow.IsEditStudentPopupOpen());

        // 9. Click Cancel Button
        _editStudentPage.Cancel();
        Assert.IsFalse(_mainWindow.IsEditStudentPopupOpen());
    }
}