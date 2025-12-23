using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Linq;
using System.Threading;

public class DeleteConfirmationPageTests : IDisposable
{
    private readonly WindowsDriver<WindowsElement> _session;
    private readonly MainWindowPage _mainWindow;
    private readonly DeleteConfirmationPage _deleteConfirmation;

    public DeleteConfirmationPageTests()
    {
        var appCapabilities = new AppiumOptions();
        appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
        appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
        _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
        _mainWindow = new MainWindowPage(_session);
        _deleteConfirmation = new DeleteConfirmationPage(_session);
    }

    [Fact]
    [Trait("TestCaseId", "75860")]
    public void DeleteConfirmation_FullWorkflow_Test()
    {
        // 1. Main window should open with Student's data grid and 3 buttons
        Assert.NotNull(_mainWindow.StudentDataGrid);
        Assert.NotNull(_mainWindow.AddNewStudentButton);
        Assert.NotNull(_mainWindow.EditStudentButton);
        Assert.NotNull(_mainWindow.DeleteStudentButton);

        // 2. Click Delete Button (no selection)
        _mainWindow.DeleteStudent();
        Assert.True(WaitUntil(() => _deleteConfirmation.SelectedStudentsList.Displayed));
        Assert.True(_deleteConfirmation.ConfirmationYesButton.Displayed);
        Assert.True(_deleteConfirmation.ConfirmationNoButton.Displayed);
        // Should be empty (actual rows)
        var items = _deleteConfirmation.SelectedStudentsList.FindElementsByClassName("ListViewItem");
        Assert.Empty(items);

        // 3. Click No button
        _deleteConfirmation.CancelDelete();
        Assert.True(WaitUntil(() => ThrowsWebDriver(() => _deleteConfirmation.SelectedStudentsList.Displayed)));

        // 4. Select "ALL" in Student Data List drop down
        _mainWindow.SectionsComboBox.Click();
        _mainWindow.SectionsComboBox.SendKeys("All");
        _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);
        Assert.True(WaitUntil(() => _mainWindow.StudentDataGrid.Displayed));

        // 5. Click checkbox of the first student
        var firstRowCheckbox = _mainWindow.StudentDataGrid.FindElementByXPath("//DataItem[1]//CheckBox");
        firstRowCheckbox.Click();
            Thread.Sleep(200); // allow UI to update
            var toggleState = firstRowCheckbox.GetAttribute("Toggle.ToggleState");
            Assert.True(toggleState == "1", $"Expected ToggleState=1 but got {toggleState}");

            // Capture row count before delete
            var beforeRows = _mainWindow.StudentDataGrid.FindElementsByXPath("//DataItem");
            var rowsCountBefore = beforeRows.Count;

            // 6. Click Delete Button
            _mainWindow.DeleteStudent();
            Assert.True(WaitUntil(() => _deleteConfirmation.SelectedStudentsList.Displayed));
            var selectedListItems = _deleteConfirmation.SelectedStudentsList.FindElementsByClassName("ListViewItem");
            Assert.NotEmpty(selectedListItems);
            // Capture the text of the to-be-deleted item BEFORE confirming
            var candidateText = selectedListItems[0].Text;

            // 7. Click Yes Button
            _deleteConfirmation.ConfirmDelete();
            Assert.True(WaitUntil(() => ThrowsWebDriver(() => _deleteConfirmation.SelectedStudentsList.Displayed)));
            // Verify student is deleted: wait until either row count decreases by 1 OR candidate text disappears
            Assert.True(WaitUntil(() =>
            {
                var rows = _mainWindow.StudentDataGrid.FindElementsByXPath("//DataItem");
                return rows.Count == rowsCountBefore - 1 || rows.All(r => !r.Text.Contains(candidateText));
            }, timeoutMs: 4000), "Expected the selected student to be deleted from the grid.");

        // 8. Click Select All Checkbox at header
        var selectAllCheckbox = _mainWindow.StudentDataGrid.FindElementByAccessibilityId("SelectAllCheckBox");
        selectAllCheckbox.Click();
        Thread.Sleep(300);
        var rowCheckboxes = _mainWindow.StudentDataGrid.FindElementsByXPath("//DataItem//CheckBox");
        foreach (var cb in rowCheckboxes)
        {
            var state = cb.GetAttribute("Toggle.ToggleState");
            Assert.True(state == "1", $"Row checkbox not checked. ToggleState={state}");
        }

        // 9. Click Delete Button
        _mainWindow.DeleteStudent();
        Assert.True(WaitUntil(() => _deleteConfirmation.SelectedStudentsList.Displayed));
        var allSelectedItems = _deleteConfirmation.SelectedStudentsList.FindElementsByClassName("ListViewItem");
        Assert.NotEmpty(allSelectedItems);

        // 10. Click Yes Button
        _deleteConfirmation.ConfirmDelete();
        Assert.True(WaitUntil(() => ThrowsWebDriver(() => _deleteConfirmation.SelectedStudentsList.Displayed)));
        // All students should be deleted
        Assert.True(WaitUntil(() =>
        {
            var remainingRows = _mainWindow.StudentDataGrid.FindElementsByXPath("//DataItem");
            return remainingRows.Count == 0;
        }, timeoutMs: 5000), "Expected all students to be deleted and the grid to be empty.");
    }

    private static bool ThrowsWebDriver(Func<bool> action)
    {
        try { _ = action(); return false; }
        catch (OpenQA.Selenium.WebDriverException) { return true; }
    }

    private static bool WaitUntil(Func<bool> condition, int timeoutMs = 3000, int pollMs = 100)
    {
        var start = Environment.TickCount;
        while (Environment.TickCount - start < timeoutMs)
        {
            try
            {
                if (condition()) return true;
            }
            catch
            {
                // swallow and retry (useful when element not yet available)
            }
            Thread.Sleep(pollMs);
        }
        return false;
    }

    public void Dispose()
    {
        _session?.Quit();
    }
}