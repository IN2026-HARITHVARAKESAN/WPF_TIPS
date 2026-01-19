using StudentDataViewer.UITests.Infrastructure;
using StudentDataViewer.UITests.PageObjects;
using Xunit;

namespace StudentDataViewer.UITests.Tests;

/// <summary>
/// Test Case ID: 75860
/// Title: [Appium Learning] Delete Functionality Test | StudentDataViewer Application Tests
/// Description: Tests the delete functionality of the StudentDataViewer application
/// </summary>
public class DeleteFunctionalityTests : BaseTest
{
    private MainWindowPage _mainWindow;
    private DeleteStudentWindowPage _deleteWindow;

    public DeleteFunctionalityTests()
    {
        _mainWindow = new MainWindowPage(Driver);
        _deleteWindow = new DeleteStudentWindowPage(Driver);
    }

    [Fact]
    [Trait("Category", "UI")]
    [Trait("TestCaseId", "75860")]
    public void DeleteFunctionality_CompleteWorkflow_ShouldWorkCorrectly()
    {
        // Test Step 1: Navigate to StudentDataViewer Application and Open it
        // Expected: Main window of the application should open with Student's data grid and 3 buttons
        Assert.True(_mainWindow.IsMainWindowDisplayed(), "Main window should be displayed");
        Assert.True(_mainWindow.IsStudentDataGridDisplayed(), "Student data grid should be displayed");
        WaitForSeconds(2);

        // Test Step 2: Click On Delete Button
        // Expected: Delete Student Window should be opened with Empty Students Data grid, Yes and No button
        _mainWindow.ClickDeleteButton();
        Assert.True(_deleteWindow.IsDeleteWindowDisplayed(), "Delete confirmation window should be displayed");
        Assert.True(_deleteWindow.IsStudentsListEmpty(), "Students list in delete window should be empty");
        WaitForSeconds(1);

        // Test Step 3: Click on No button in the Delete Student Window
        // Expected: Delete Student window should be closed
        _deleteWindow.ClickNoButton();
        WaitForSeconds(1);
        Assert.True(_deleteWindow.IsWindowClosed(), "Delete window should be closed after clicking No");

        // Test Step 4: Click on Student Data List drop down and select "ALL"
        // Expected: All students' details should be displayed in the students data grid below
        _mainWindow.SelectSection("ALL");
        int initialStudentCount = _mainWindow.GetStudentRowCount();
        Assert.True(initialStudentCount > 0, "Student data grid should display all students");
        WaitForSeconds(1);

        // Test Step 5: Click checkbox of the first student in the Students data table
        // Expected: The clicked checkbox should be checked
        _mainWindow.SelectStudentCheckboxByIndex(0);
        WaitForSeconds(1);

        // Test Step 6: Click on the delete button
        // Expected: Delete Student Window should be opened, and the details of the checked student should be shown
        _mainWindow.ClickDeleteButton();
        Assert.True(_deleteWindow.IsDeleteWindowDisplayed(), "Delete window should be displayed");
        Assert.Equal(1, _deleteWindow.GetSelectedStudentsCount());
        WaitForSeconds(1);

        // Test Step 7: Click Yes Button
        // Expected: Delete Student Window should be closed, student should be deleted from the data
        _deleteWindow.ClickYesButton();
        WaitForSeconds(2);
        Assert.True(_deleteWindow.IsWindowClosed(), "Delete window should be closed after confirmation");
        
        int studentCountAfterDelete = _mainWindow.GetStudentRowCount();
        Assert.Equal(initialStudentCount - 1, studentCountAfterDelete);
        WaitForSeconds(1);

        // Test Step 8: Click of Select All Checkbox at the header of the table
        // Expected: Check box of all the students in the table should be checked
        _mainWindow.ClickSelectAllCheckbox();
        WaitForSeconds(1);

        // Test Step 9: Click delete button
        // Expected: Delete Student window should be shown with the details of all the students
        _mainWindow.ClickDeleteButton();
        Assert.True(_deleteWindow.IsDeleteWindowDisplayed(), "Delete window should be displayed");
        int remainingStudents = studentCountAfterDelete;
        Assert.Equal(remainingStudents, _deleteWindow.GetSelectedStudentsCount());
        WaitForSeconds(1);

        // Test Step 10: Click Yes button
        // Expected: Delete student window should be closed, all students should be deleted from the list
        _deleteWindow.ClickYesButton();
        WaitForSeconds(2);
        Assert.True(_deleteWindow.IsWindowClosed(), "Delete window should be closed");
        
        int finalStudentCount = _mainWindow.GetStudentRowCount();
        Assert.Equal(0, finalStudentCount);
    }

    [Fact]
    [Trait("Category", "UI")]
    [Trait("TestCaseId", "75860")]
    public void DeleteFunctionality_CancelDeletion_ShouldNotDeleteStudent()
    {
        // Arrange: Open application and verify main window
        Assert.True(_mainWindow.IsMainWindowDisplayed(), "Main window should be displayed");
        
        // Act: Select all students and click delete
        _mainWindow.SelectSection("ALL");
        WaitForSeconds(1);
        int initialCount = _mainWindow.GetStudentRowCount();
        
        _mainWindow.SelectStudentCheckboxByIndex(0);
        _mainWindow.ClickDeleteButton();
        
        // Act: Cancel deletion
        Assert.True(_deleteWindow.IsDeleteWindowDisplayed(), "Delete window should be displayed");
        _deleteWindow.ClickNoButton();
        WaitForSeconds(1);
        
        // Assert: Student count should remain unchanged
        int finalCount = _mainWindow.GetStudentRowCount();
        Assert.Equal(initialCount, finalCount);
    }

    [Fact]
    [Trait("Category", "UI")]
    [Trait("TestCaseId", "75860")]
    public void DeleteFunctionality_DeleteSingleStudent_ShouldRemoveOnlySelectedStudent()
    {
        // Arrange: Open application and verify main window
        Assert.True(_mainWindow.IsMainWindowDisplayed(), "Main window should be displayed");
        
        // Act: Select a student and delete
        _mainWindow.SelectSection("ALL");
        WaitForSeconds(1);
        int initialCount = _mainWindow.GetStudentRowCount();
        
        _mainWindow.SelectStudentCheckboxByIndex(0);
        _mainWindow.ClickDeleteButton();
        
        // Assert: Verify only one student is selected for deletion
        Assert.True(_deleteWindow.IsDeleteWindowDisplayed(), "Delete window should be displayed");
        Assert.Equal(1, _deleteWindow.GetSelectedStudentsCount());
        
        // Act: Confirm deletion
        _deleteWindow.ClickYesButton();
        WaitForSeconds(2);
        
        // Assert: Verify student count decreased by 1
        int finalCount = _mainWindow.GetStudentRowCount();
        Assert.Equal(initialCount - 1, finalCount);
    }
}
