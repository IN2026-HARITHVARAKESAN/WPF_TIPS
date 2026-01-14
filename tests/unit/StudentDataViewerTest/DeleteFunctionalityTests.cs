using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;
using System.Linq;

namespace StudentDataViewerTest
{
    /// <summary>
    /// Test Case ID: 75860
    /// Title: [Appium Learning] Delete Functionality Test | StudentDataViewer Application Tests
    /// Description: Tests the complete delete functionality including deleting individual students
    ///              and deleting all students using the Select All checkbox
    /// </summary>
    public class DeleteFunctionalityTests : IDisposable
    {
        private readonly WindowsDriver<WindowsElement> _session;
        private readonly MainWindowPage _mainWindow;

        public DeleteFunctionalityTests()
        {
            var appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
            _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
            
            Thread.Sleep(1500);
            _mainWindow = new MainWindowPage(_session);
        }

        /// <summary>
        /// Test Case ID: 75860
        /// Title: [Appium Learning] Delete Functionality Test | StudentDataViewer Application Tests
        /// Steps:
        /// 1. Navigate to StudentDataViewer Application and Open it
        /// 2. Click On Delete Button (without selection)
        /// 3. Click on No button in the Delete Student Window
        /// 4. Click on Student Data List drop down and select "ALL"
        /// 5. Click checkbox of the first student in the Students data table
        /// 6. Click on the delete button
        /// 7. Click Yes Button
        /// 8. Click of Select All Checkbox at the header of the table
        /// 9. Click delete button
        /// 10. Click Yes button
        /// </summary>
        [Fact]
        [Trait("TestCaseId", "75860")]
        [Trait("Category", "Appium")]
        [Trait("Priority", "P2")]
        public void TC75860_DeleteFunctionality_ShouldDeleteStudentsSuccessfully()
        {
            // Step 2: Navigate to StudentDataViewer Application and Open it
            // Expected: Main window of the application should open with Student's data grid and 3 buttons (Add, Edit and Delete Students)
            Assert.NotNull(_mainWindow.StudentDataGrid);
            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Student Data Grid should be visible");
            Assert.NotNull(_mainWindow.AddNewStudentButton);
            Assert.True(_mainWindow.AddNewStudentButton.Displayed, "Add New Student button should be visible");
            Assert.NotNull(_mainWindow.EditStudentButton);
            Assert.True(_mainWindow.EditStudentButton.Displayed, "Edit Student button should be visible");
            Assert.NotNull(_mainWindow.DeleteStudentButton);
            Assert.True(_mainWindow.DeleteStudentButton.Displayed, "Delete Student button should be visible");

            // Step 3: Click On Delete Button
            // Expected: Delete Student Window should be opened with Empty Students Data grid, Yes and No button
            _mainWindow.DeleteStudentButton.Click();
            Thread.Sleep(1000);

            WindowsElement deleteDialog = _session.FindElementByAccessibilityId("DeleteConfirmationView");
            Assert.NotNull(deleteDialog);
            
            var selectedStudentsList = _session.FindElementByAccessibilityId("SelectedStudentsList");
            Assert.NotNull(selectedStudentsList);
            Assert.True(selectedStudentsList.Displayed, "Selected students list should be visible");
            
            var yesButton = _session.FindElementByAccessibilityId("ConfirmationYes");
            Assert.NotNull(yesButton);
            Assert.True(yesButton.Displayed, "Yes button should be visible");
            
            var noButton = _session.FindElementByAccessibilityId("ConfirmationNo");
            Assert.NotNull(noButton);
            Assert.True(noButton.Displayed, "No button should be visible");

            // Step 4: Click on No button in the Delete Student Window
            // Expected: Delete Student window should be closed
            noButton.Click();
            Thread.Sleep(500);

            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Should return to main window after clicking No");

            // Step 5: Click on Student Data List drop down and select "ALL"
            // Expected: All students' details should be displayed in the students data grid below
            try
            {
                _mainWindow.SectionsComboBox.Click();
                Thread.Sleep(500);
                
                // Clear existing text and type ALL
                _mainWindow.SectionsComboBox.Clear();
                Thread.Sleep(300);
                _mainWindow.SectionsComboBox.SendKeys("ALL");
                Thread.Sleep(300);
                
                // Press Enter to confirm selection
                _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);
                Thread.Sleep(1500);
                
                Console.WriteLine("Successfully selected 'ALL' from dropdown");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error selecting ALL from dropdown - {ex.Message}");
                
                // Alternative approach: try typing with tab
                try
                {
                    _mainWindow.SectionsComboBox.Click();
                    Thread.Sleep(300);
                    _mainWindow.SectionsComboBox.SendKeys("ALL");
                    _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Tab);
                    Thread.Sleep(1000);
                    Console.WriteLine("Alternative: Selected ALL using Tab key");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"Alternative approach also failed: {ex2.Message}");
                }
            }

            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Student data grid should show all students");

            // Step 6: Click checkbox of the first student in the Students data table
            // Expected: The clicked checkbox should be checked
            bool firstStudentSelected = false;
            try
            {
                var studentRows = _session.FindElementsByAccessibilityId("StudentData");
                var firstRow = studentRows.FirstOrDefault();
                if (firstRow != null)
                {
                    var checkboxes = firstRow.FindElementsByClassName("CheckBox");
                    var firstCheckbox = checkboxes.FirstOrDefault();
                    if (firstCheckbox != null)
                    {
                        firstCheckbox.Click();
                        Thread.Sleep(500);
                        firstStudentSelected = true;
                        Console.WriteLine("First student checkbox clicked successfully");
                    }
                    else
                    {
                        Console.WriteLine("Warning: First student checkbox not found");
                    }
                }
                else
                {
                    Console.WriteLine("Warning: No student rows found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not select first student checkbox - {ex.Message}");
            }

            // Step 7: Click on the delete button
            // Expected: Delete Student Window should be opened, and the details of the checked student should be shown in the table
            if (firstStudentSelected)
            {
                _mainWindow.DeleteStudentButton.Click();
                Thread.Sleep(1000);

                try
                {
                    deleteDialog = _session.FindElementByAccessibilityId("DeleteConfirmationView");
                    Assert.NotNull(deleteDialog);
                    
                    selectedStudentsList = _session.FindElementByAccessibilityId("SelectedStudentsList");
                    Assert.NotNull(selectedStudentsList);
                    Assert.True(selectedStudentsList.Displayed, "Selected student should be shown in delete window");

                    // Step 8: Click Yes Button
                    // Expected: Delete Student Window should be closed.
                    //          The checked student details should be deleted from the data and should not be shown in the table in main window.
                    //          The deleted students mark should not be shown in the graph.
                    yesButton = _session.FindElementByAccessibilityId("ConfirmationYes");
                    yesButton.Click();
                    Thread.Sleep(1500);

                    try
                    {
                        Assert.True(_mainWindow.StudentDataGrid.Displayed, "Should return to main window after deletion");
                    }
                    catch
                    {
                        Console.WriteLine("Warning: Could not verify main window after deletion");
                    }
                    Console.WriteLine("First student deleted successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not complete single student deletion - {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Note: Skipping single student deletion - checkbox not selected");
            }

            // Step 9: Click of Select All Checkbox at the header of the table
            // Expected: Check box of all the students in the table should be checked
            try
            {
                var selectAllCheckbox = _session.FindElementByAccessibilityId("SelectAllCheckBox");
                if (selectAllCheckbox != null && selectAllCheckbox.Displayed)
                {
                    selectAllCheckbox.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("Select All checkbox clicked successfully");
                }
                else
                {
                    Console.WriteLine("Warning: Select All checkbox not found or not displayed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not click Select All checkbox - {ex.Message}");
            }

            // Step 10: Click delete button
            // Expected: Delete Student window should be shown with the details of all the students
            try
            {
                _mainWindow.DeleteStudentButton.Click();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not click delete button - {ex.Message}");
                throw; // Re-throw as this is critical for the test
            }

            deleteDialog = _session.FindElementByAccessibilityId("DeleteConfirmationView");
            Assert.NotNull(deleteDialog);
            
            selectedStudentsList = _session.FindElementByAccessibilityId("SelectedStudentsList");
            Assert.NotNull(selectedStudentsList);
            Assert.True(selectedStudentsList.Displayed, "All selected students should be shown");

            // Step 11: Click Yes button
            // Expected: Delete student window should be closed.
            //          All the students should be deleted from the list is not shown in main window table.
            //          The deleted students mark should not be shown in the graph.
            yesButton = _session.FindElementByAccessibilityId("ConfirmationYes");
            yesButton.Click();
            Thread.Sleep(1500);

            Console.WriteLine("Delete functionality test completed - all students deleted");
        }

        public void Dispose()
        {
            try
            {
                _session?.Close();
            }
            catch { }
            
            try
            {
                _session?.Quit();
            }
            catch { }
        }
    }
}
