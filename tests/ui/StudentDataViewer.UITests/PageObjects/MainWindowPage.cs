using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace StudentDataViewer.UITests.PageObjects;

public class MainWindowPage : BasePage
{
    public MainWindowPage(WindowsDriver<WindowsElement> driver) : base(driver)
    {
    }

    // Window elements
    private By WindowTitle => By.Name("Student Data Viewer");
    private By MinimizeButton => By.Name("MinimizeButton");
    private By CloseButton => By.Name("CloseButton");

    // Section selector
    private By SectionsComboBox => ByAccessibilityId("SectionsComboBox");

    // Student Data Grid
    private By StudentDataGrid => ByAccessibilityId("StudentDataGrid");
    private By SelectAllCheckBox => ByAccessibilityId("SelectAllCheckBox");

    // Action buttons
    private By AddNewStudentButton => ByAccessibilityId("AddNewStudentButton");
    private By EditStudentButton => ByAccessibilityId("EditStudentButton");
    private By DeleteStudentButton => ByAccessibilityId("DeleteStudentButton");

    // Methods
    public bool IsMainWindowDisplayed()
    {
        try
        {
            WaitForElementToBeVisible(WindowTitle);
            return IsElementDisplayed(WindowTitle);
        }
        catch
        {
            return false;
        }
    }

    public void SelectSection(string sectionName)
    {
        Click(SectionsComboBox);
        var sectionItem = By.Name(sectionName);
        Click(sectionItem);
        WaitForSeconds(1); // Wait for grid to refresh
    }

    public bool IsStudentDataGridDisplayed()
    {
        return IsElementDisplayed(StudentDataGrid);
    }

    public void SelectStudentCheckboxByIndex(int index)
    {
        var grid = FindElement(StudentDataGrid);
        var rows = grid.FindElements(By.ClassName("DataGridRow"));
        
        if (index >= 0 && index < rows.Count)
        {
            var row = rows[index];
            var checkbox = row.FindElement(By.ClassName("CheckBox"));
            checkbox.Click();
            WaitForSeconds(1);
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Row index {index} is out of range. Total rows: {rows.Count}");
        }
    }

    public void ClickSelectAllCheckbox()
    {
        Click(SelectAllCheckBox);
        WaitForSeconds(1);
    }

    public bool IsSelectAllCheckboxChecked()
    {
        var checkbox = FindElement(SelectAllCheckBox);
        return checkbox.Selected;
    }

    public void ClickDeleteButton()
    {
        Click(DeleteStudentButton);
        WaitForSeconds(1);
    }

    public void ClickAddButton()
    {
        Click(AddNewStudentButton);
        WaitForSeconds(1);
    }

    public void ClickEditButton()
    {
        Click(EditStudentButton);
        WaitForSeconds(1);
    }

    public int GetStudentRowCount()
    {
        try
        {
            var grid = FindElement(StudentDataGrid);
            var rows = grid.FindElements(By.ClassName("DataGridRow"));
            return rows.Count;
        }
        catch
        {
            return 0;
        }
    }

    public void CloseWindow()
    {
        Click(CloseButton);
    }

    private void WaitForSeconds(int seconds)
    {
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }
}
