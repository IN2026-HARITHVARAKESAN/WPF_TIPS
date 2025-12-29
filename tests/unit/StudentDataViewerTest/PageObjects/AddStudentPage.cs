using OpenQA.Selenium.Appium.Windows;

public class AddStudentPage
{
    private readonly WindowsDriver<WindowsElement> _session;

    public AddStudentPage(WindowsDriver<WindowsElement> session)
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

    public WindowsElement MarkTextBox =>
        _session.FindElementByAccessibilityId("Mark");

    public WindowsElement AddStudentButton =>
        _session.FindElementByAccessibilityId("AddStudentButton");

    public WindowsElement CancelButton =>
        _session.FindElementByAccessibilityId("AddStudentCancelButton");

    public WindowsElement CloseButton =>
        _session.FindElementByAccessibilityId("AddStudentCloseButton");

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

    public void FillStudentDetails(string name, string department, string section, string year, string cgpa, string mark)
    {
        FillStudentDetails(name, department, section, year, cgpa);
        MarkTextBox.Clear();
        MarkTextBox.SendKeys(mark);
    }

    public void AddStudent() => AddStudentButton.Click();

    public void Cancel() => CancelButton.Click();

    public void Close() => CloseButton.Click();
}