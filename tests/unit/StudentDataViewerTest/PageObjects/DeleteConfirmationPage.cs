using OpenQA.Selenium.Appium.Windows;

public class DeleteConfirmationPage
{
    private readonly WindowsDriver<WindowsElement> _session;

    public DeleteConfirmationPage(WindowsDriver<WindowsElement> session)
    {
        _session = session;
    }

    public WindowsElement SelectedStudentsList =>
        _session.FindElementByAccessibilityId("SelectedStudentsList");

    public WindowsElement ConfirmationYesButton =>
        _session.FindElementByAccessibilityId("ConfirmationYes");

    public WindowsElement ConfirmationNoButton =>
        _session.FindElementByAccessibilityId("ConfirmationNo");

    public void ConfirmDelete() => ConfirmationYesButton.Click();

    public void CancelDelete() => ConfirmationNoButton.Click();
}