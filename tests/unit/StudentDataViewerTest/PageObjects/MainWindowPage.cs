using OpenQA.Selenium.Appium.Windows;

public class MainWindowPage
{
    private readonly WindowsDriver<WindowsElement> _session;

    public MainWindowPage(WindowsDriver<WindowsElement> session)
    {
        _session = session;
    }

    public WindowsElement MinimizeButton =>
        _session.FindElementByAccessibilityId("MinimizeButton");

    public WindowsElement CloseButton =>
        _session.FindElementByAccessibilityId("CloseButton");

    public WindowsElement SectionsComboBox =>
        _session.FindElementByAccessibilityId("SectionsComboBox");

    public WindowsElement StudentDataGrid =>
        _session.FindElementByAccessibilityId("StudentDataGrid");

    public WindowsElement AddNewStudentButton =>
        _session.FindElementByAccessibilityId("AddNewStudentButton");

    public WindowsElement EditStudentButton =>
        _session.FindElementByAccessibilityId("EditStudentButton");

    public WindowsElement DeleteStudentButton =>
        _session.FindElementByAccessibilityId("DeleteStudentButton");

    public void Minimize() => MinimizeButton.Click();

    public void Close() => CloseButton.Click();

    public void AddNewStudent() => AddNewStudentButton.Click();

    public void EditStudent() => EditStudentButton.Click();

    public void DeleteStudent() => DeleteStudentButton.Click();
}