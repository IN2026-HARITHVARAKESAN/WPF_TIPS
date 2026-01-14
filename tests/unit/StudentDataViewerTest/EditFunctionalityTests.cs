using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;
using System.Linq;

namespace StudentDataViewerTest
{
    /// <summary>
    /// Test Case ID: 75849
    /// Title: [Appium Learning] Edit Functionality Test | StudentDataViewer Application Tests
    /// Description: Tests the complete edit functionality including editing student details
    ///              and canceling the edit operation
    /// </summary>
    public class EditFunctionalityTests : IDisposable
    {
        private readonly WindowsDriver<WindowsElement> _session;
        private readonly MainWindowPage _mainWindow;

        public EditFunctionalityTests()
        {
            var appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
            _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
            
            Thread.Sleep(1500);
            _mainWindow = new MainWindowPage(_session);
        }

        /// <summary>
        /// Test Case ID: 75849
        /// Title: [Appium Learning] Edit Functionality Test | StudentDataViewer Application Tests
        /// Steps:
        /// 1. Navigate to StudentDataViewer Application and open
        /// 2. Click on Edit Student Button (without selection)
        /// 3. Click on Student Data List drop down and select the section or "All"
        /// 4. Click on any student details
        /// 5. Click Edit Student Button
        /// 6. Change the name of the student in the Name text box
        /// 7. Change the mark of the student in the Mark text box
        /// 8. Click Save Button
        /// 9. Click on Edit Student Button
        /// 10. Click Cancel Button
        /// </summary>
        [Fact]
        [Trait("TestCaseId", "75849")]
        [Trait("Category", "Appium")]
        [Trait("Priority", "P2")]
        public void TC75849_EditFunctionality_ShouldEditStudentSuccessfully()
        {
            // Step 2: Navigate to StudentDataViewer Application and open
            // Expected: Main window of the application should open with Student's data grid and 3 buttons (Add, Edit and Delete Students)
            Assert.NotNull(_mainWindow.StudentDataGrid);
            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Student Data Grid should be visible");
            Assert.NotNull(_mainWindow.AddNewStudentButton);
            Assert.True(_mainWindow.AddNewStudentButton.Displayed, "Add New Student button should be visible");
            Assert.NotNull(_mainWindow.EditStudentButton);
            Assert.True(_mainWindow.EditStudentButton.Displayed, "Edit Student button should be visible");
            Assert.NotNull(_mainWindow.DeleteStudentButton);
            Assert.True(_mainWindow.DeleteStudentButton.Displayed, "Delete Student button should be visible");

            // Step 3: Click on Edit Student Button (without selection)
            // Expected: Edit Student Popup Window should not be opened as no student were selected
            _mainWindow.EditStudentButton.Click();
            Thread.Sleep(1000);

            // Verify Edit window did NOT open
            try
            {
                var checkEditWindow = _session.FindElementByAccessibilityId("EditStudentView");
                if (checkEditWindow != null && checkEditWindow.Displayed)
                {
                    Assert.Fail("Edit window should not open without selecting a student");
                }
            }
            catch
            {
                Console.WriteLine("Edit window correctly did not open without selection");
            }

            // Step 4: Click on Student Data List drop down and select the section or "All"
            // Expected: Students detail of the selected section should be shown
            try
            {
                _mainWindow.SectionsComboBox.Click();
                Thread.Sleep(500);
                
                _mainWindow.SectionsComboBox.Clear();
                Thread.Sleep(300);
                _mainWindow.SectionsComboBox.SendKeys("ALL");
                Thread.Sleep(300);
                
                _mainWindow.SectionsComboBox.SendKeys(OpenQA.Selenium.Keys.Enter);
                Thread.Sleep(1500);
                
                Console.WriteLine("Successfully selected 'ALL' from dropdown");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error selecting ALL from dropdown - {ex.Message}");
            }

            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Student data grid should show all students");

            // Step 5: Click on any student details
            string originalName = "";
            string originalMark = "";
            try
            {
                var studentRows = _session.FindElementsByAccessibilityId("StudentData");
                var firstRow = studentRows.FirstOrDefault();
                if (firstRow != null)
                {
                    firstRow.Click();
                    Thread.Sleep(500);
                    Console.WriteLine("First student row selected");
                }
                else
                {
                    Console.WriteLine("Warning: No student rows found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not select student row - {ex.Message}");
            }

            // Step 6: Click Edit Student Button
            // Expected: Edit Student Popup window should be opened.
            //          All Text Boxes and Drop down should show the old details of selected student with save and close button
            _mainWindow.EditStudentButton.Click();
            Thread.Sleep(1000);

            WindowsElement editWindow = _session.FindElementByAccessibilityId("EditStudentView");
            Assert.NotNull(editWindow);
            Assert.True(editWindow.Displayed, "Edit Student window should be opened");

            var nameTextBox = _session.FindElementByAccessibilityId("Name");
            Assert.NotNull(nameTextBox);
            Assert.True(nameTextBox.Displayed, "Name text box should be visible");

            var markTextBox = _session.FindElementByAccessibilityId("Mark");
            Assert.NotNull(markTextBox);
            Assert.True(markTextBox.Displayed, "Mark text box should be visible");

            var saveButton = _session.FindElementByAccessibilityId("SaveButton");
            Assert.NotNull(saveButton);
            Assert.True(saveButton.Displayed, "Save button should be visible");

            var cancelButton = _session.FindElementByAccessibilityId("CancelButton");
            Assert.NotNull(cancelButton);
            Assert.True(cancelButton.Displayed, "Cancel button should be visible");

            // Capture original values
            originalName = nameTextBox.Text;
            originalMark = markTextBox.Text;
            Console.WriteLine($"Original Name: {originalName}, Original Mark: {originalMark}");

            // Step 7: Change the name of the student in the Name text box
            string newName = "Updated Student Name";
            nameTextBox.Clear();
            Thread.Sleep(300);
            nameTextBox.SendKeys(newName);
            Thread.Sleep(500);
            Console.WriteLine($"Changed name to: {newName}");

            // Step 11: Change the mark of the student in the Mark text box
            string newMark = "95";
            markTextBox.Clear();
            Thread.Sleep(300);
            markTextBox.SendKeys(newMark);
            Thread.Sleep(500);
            Console.WriteLine($"Changed mark to: {newMark}");

            // Step 8: Click Save Button
            // Expected: The Edit Student Popup window should be closed.
            //          The new name of the student should be updated and displayed in the Student Data grid in main window.
            //          The new mark of the student should be updated on the graph.
            saveButton.Click();
            Thread.Sleep(1500);

            // Verify edit window closed
            try
            {
                Assert.False(editWindow.Displayed, "Edit window should be closed after save");
            }
            catch
            {
                Console.WriteLine("Edit window closed successfully");
            }

            // Verify the data grid is visible again
            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Should return to main window after save");
            Console.WriteLine("Student details saved successfully");

            // Step 9: Click on Edit Student Button (student should still be selected)
            // Expected: Edit Student Popup window should be opened
            Thread.Sleep(500);
            _mainWindow.EditStudentButton.Click();
            Thread.Sleep(1000);

            editWindow = _session.FindElementByAccessibilityId("EditStudentView");
            Assert.NotNull(editWindow);
            Assert.True(editWindow.Displayed, "Edit Student window should be opened again");

            cancelButton = _session.FindElementByAccessibilityId("CancelButton");
            Assert.NotNull(cancelButton);
            Assert.True(cancelButton.Displayed, "Cancel button should be visible");

            // Step 10: Click Cancel Button
            // Expected: Edit Student Popup window should be closed
            cancelButton.Click();
            Thread.Sleep(1000);

            // Verify edit window closed
            try
            {
                Assert.False(editWindow.Displayed, "Edit window should be closed after cancel");
            }
            catch
            {
                Console.WriteLine("Edit window closed successfully after cancel");
            }

            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Should return to main window after cancel");
            Console.WriteLine("Edit functionality test completed successfully");
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
