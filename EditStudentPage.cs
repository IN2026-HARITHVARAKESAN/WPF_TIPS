using OpenQA.Selenium.Appium.Windows;

public class EditStudentPage
{
    private readonly WindowsDriver<WindowsElement> _session;

    public EditStudentPage(WindowsDriver<WindowsElement> session)
    {
        _session = session;
    }

    private WindowsElement StudentIdTextBox =>
        _session.FindElementByAccessibilityId("StudentId");

    private WindowsElement NameTextBox =>
        _session.FindElementByAccessibilityId("Name");

    private WindowsElement DepartmentComboBox =>
        _session.FindElementByAccessibilityId("Department");

    private WindowsElement SectionComboBox =>
        _session.FindElementByAccessibilityId("Section");

    private WindowsElement YearComboBox =>
        _session.FindElementByAccessibilityId("Year");

    private WindowsElement CGPATextBox =>
        _session.FindElementByAccessibilityId("CGPA");

    private WindowsElement AddStudentButton =>
        _session.FindElementByAccessibilityId("AddStudentButton");

    private WindowsElement CancelButton =>
        _session.FindElementByAccessibilityId("CancelButton");

    private WindowsElement CloseButton =>
        _session.FindElementByAccessibilityId("CloseButton");

    public void SetName(string name)
    {
        NameTextBox.Clear();
        NameTextBox.SendKeys(name);
    }

    public void SetDepartment(string department)
    {
        DepartmentComboBox.Click();
        DepartmentComboBox.SendKeys(department);
        DepartmentComboBox.SendKeys(Keys.Enter);
    }

    public void SetSection(string section)
    {
        SectionComboBox.Click();
        SectionComboBox.SendKeys(section);
        SectionComboBox.SendKeys(Keys.Enter);
    }

    public void SetYear(string year)
    {
        YearComboBox.Click();
        YearComboBox.SendKeys(year);
        YearComboBox.SendKeys(Keys.Enter);
    }

    public void SetCGPA(string cgpa)
    {
        CGPATextBox.Clear();
        CGPATextBox.SendKeys(cgpa);
    }

    public void Save() => AddStudentButton.Click();

    public void Cancel() => CancelButton.Click();

    public void Close() => CloseButton.Click();

    public string GetStudentId() => StudentIdTextBox.Text;
}