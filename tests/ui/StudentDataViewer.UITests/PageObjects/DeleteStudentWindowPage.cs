using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace StudentDataViewer.UITests.PageObjects;

public class DeleteStudentWindowPage : BasePage
{
    public DeleteStudentWindowPage(WindowsDriver<WindowsElement> driver) : base(driver)
    {
    }

    // Window elements
    private By DeleteConfirmationWindow => ByAccessibilityId("DeleteConfirmationView");
    private By ConfirmationMessage => By.Name("Are you sure you want to delete this student?");

    // Student List in delete window
    private By SelectedStudentsList => ByAccessibilityId("SelectedStudentsList");

    // Action buttons
    private By ConfirmationYesButton => ByAccessibilityId("ConfirmationYes");
    private By ConfirmationNoButton => ByAccessibilityId("ConfirmationNo");

    // Methods
    public bool IsDeleteWindowDisplayed()
    {
        try
        {
            WaitForWindowToLoad("DeleteConfirmationView", 5);
            return IsElementDisplayed(DeleteConfirmationWindow) || IsElementDisplayed(ConfirmationMessage);
        }
        catch
        {
            return false;
        }
    }

    public bool IsStudentsListEmpty()
    {
        try
        {
            var listView = FindElement(SelectedStudentsList);
            var items = listView.FindElements(By.ClassName("ListViewItem"));
            return items.Count == 0;
        }
        catch
        {
            return true;
        }
    }

    public int GetSelectedStudentsCount()
    {
        try
        {
            var listView = FindElement(SelectedStudentsList);
            var items = listView.FindElements(By.ClassName("ListViewItem"));
            return items.Count;
        }
        catch
        {
            return 0;
        }
    }

    public List<string> GetSelectedStudentNames()
    {
        var names = new List<string>();
        try
        {
            var listView = FindElement(SelectedStudentsList);
            var items = listView.FindElements(By.ClassName("ListViewItem"));

            foreach (var item in items)
            {
                var nameCell = item.FindElements(By.ClassName("TextBlock"));
                if (nameCell.Count > 1)
                {
                    names.Add(nameCell[1].Text);
                }
            }
        }
        catch
        {
            // Return empty list if unable to get names
        }

        return names;
    }

    public void ClickYesButton()
    {
        Click(ConfirmationYesButton);
        WaitForSeconds(1);
    }

    public void ClickNoButton()
    {
        Click(ConfirmationNoButton);
        WaitForSeconds(1);
    }

    public bool IsWindowClosed()
    {
        try
        {
            return !IsElementDisplayed(DeleteConfirmationWindow);
        }
        catch
        {
            return true; // If element not found, window is closed
        }
    }

    private void WaitForSeconds(int seconds)
    {
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }
}
