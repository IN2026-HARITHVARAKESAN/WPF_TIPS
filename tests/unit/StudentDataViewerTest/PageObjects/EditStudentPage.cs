using OpenQA.Selenium.Appium.Windows;

public class EditStudentPage
{
    private readonly WindowsDriver<WindowsElement> _session;

    public EditStudentPage(WindowsDriver<WindowsElement> session)
    {
        _session = session;
    }

    public WindowsElement StudentIdTextBox =>
        _session.FindElementByAccessibilityId("StudentId");

    public WindowsElement NameTextBox =>
        _session.FindElementByAccessibilityId("Name");

    public WindowsElement DepartmentComboBox =>
        _session.FindElementByAccessibilityId("Department");

    public WindowsElement SectionComboBox =>
        _session.FindElementByAccessibilityId("Section");

    public WindowsElement YearComboBox =>
        _session.FindElementByAccessibilityId("Year");

    public WindowsElement CGPATextBox =>
        _session.FindElementByAccessibilityId("CGPA");

    public WindowsElement SaveButton =>
        _session.FindElementByAccessibilityId("SaveButton");

    public WindowsElement CancelButton =>
        _session.FindElementByAccessibilityId("CancelButton");

    public WindowsElement CloseButton =>
        _session.FindElementByAccessibilityId("EditStudentCloseButton");

    public void FillStudentDetails(string name, string department, string section, string year, string cgpa)
    {
        NameTextBox.Clear();
        NameTextBox.SendKeys(name);

        DepartmentComboBox.Click();
        DepartmentComboBox.SendKeys(department);

        SectionComboBox.Click();
        SectionComboBox.SendKeys(section);

        YearComboBox.Click();
        YearComboBox.SendKeys(year);

        CGPATextBox.Clear();
        CGPATextBox.SendKeys(cgpa);
    }

    public void Save() => SaveButton.Click();

    public void Cancel() => CancelButton.Click();

    public void Close() => CloseButton.Click();
}