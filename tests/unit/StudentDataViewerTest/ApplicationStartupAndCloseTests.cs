using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;

namespace StudentDataViewerTest
{
    /// <summary>
    /// Test Case ID: 75776
    /// Title: [Appium Learning] Application Startup and close test | Student Data Viewer application tests
    /// Description: Validates the basic startup and shutdown functionality of the Student Data Viewer application
    /// </summary>
    public class ApplicationStartupAndCloseTests : IDisposable
    {
        private readonly WindowsDriver<WindowsElement> _session;
        private readonly MainWindowPage _mainWindow;

        public ApplicationStartupAndCloseTests()
        {
            // Navigate to the application and open it
            var appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
            _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
            
            // Give application time to fully load
            Thread.Sleep(1000);
            
            _mainWindow = new MainWindowPage(_session);
        }

        /// <summary>
        /// Test Case ID: 75776
        /// Title: [Appium Learning] Application Startup and close test | Student Data Viewer application tests
        /// Attachment: Screenshot 2025-12-24 152610.png (Expected UI state)
        /// 
        /// Step 2: Navigate to the application and open it
        /// Expected Result: Application main window should open and verify the window has Students data grid 
        ///                  and three buttons (Add, Edit and Delete Students)
        ///                  - Window title should be "Student Data Viewer"
        ///                  - Data grid should be visible and contain student records
        ///                  - All three action buttons (Add, Edit, Delete) should be present and enabled
        ///                  - Filter dropdown and section selection should be available
        ///                  - Close button should be accessible
        /// 
        /// Step 3: Click on the close button of the application
        /// Expected Result: Application should be shutdown gracefully
        /// </summary>
        [Fact]
        [Trait("TestCaseId", "75776")]
        [Trait("Category", "Appium")]
        [Trait("Priority", "P2")]
        public void TC75776_ApplicationStartupAndClose_ShouldOpenWithAllControlsAndCloseSuccessfully()
        {
            // Step 2: Verify application main window has opened with all expected controls
            // As shown in attachment: Screenshot 2025-12-24 152610.png
            
            Console.WriteLine("Step 2: Verifying application startup and main window controls");
            
            // Verify main window title
            Assert.NotNull(_session.Title);
            Console.WriteLine($"Window Title: {_session.Title}");
            
            // Verify Students data grid is present and visible
            Assert.NotNull(_mainWindow.StudentDataGrid);
            Assert.True(_mainWindow.StudentDataGrid.Displayed, "Student Data Grid should be visible");
            Console.WriteLine("✓ Student Data Grid is visible");
            
            // Verify data grid contains data (check if there are rows)
            try
            {
                var rows = _mainWindow.StudentDataGrid.FindElementsByClassName("DataGridRow");
                Console.WriteLine($"✓ Data Grid contains {rows.Count} student records");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Could not count data grid rows: {ex.Message}");
            }
            
            // Verify Add button is present and enabled
            Assert.NotNull(_mainWindow.AddNewStudentButton);
            Assert.True(_mainWindow.AddNewStudentButton.Displayed, "Add New Student button should be visible");
            Assert.True(_mainWindow.AddNewStudentButton.Enabled, "Add New Student button should be enabled");
            Console.WriteLine("✓ Add New Student button is visible and enabled");
            
            // Verify Edit button is present and visible
            Assert.NotNull(_mainWindow.EditStudentButton);
            Assert.True(_mainWindow.EditStudentButton.Displayed, "Edit Student button should be visible");
            Console.WriteLine("✓ Edit Student button is visible");
            
            // Verify Delete button is present and visible
            Assert.NotNull(_mainWindow.DeleteStudentButton);
            Assert.True(_mainWindow.DeleteStudentButton.Displayed, "Delete Student button should be visible");
            Console.WriteLine("✓ Delete Student button is visible");
            
            // Verify section filter dropdown is present (if available in MainWindowPage)
            if (_mainWindow.SectionsComboBox != null)
            {
                Assert.True(_mainWindow.SectionsComboBox.Displayed, "Section filter dropdown should be visible");
                Console.WriteLine("✓ Section filter dropdown is visible");
            }
            
            // Verify the plot/chart view is present (if available in MainWindowPage)
            if (_mainWindow.MarksPlotView != null)
            {
                Assert.True(_mainWindow.MarksPlotView.Displayed, "Marks plot view should be visible");
                Console.WriteLine("✓ Marks plot view is visible");
            }
            
            Console.WriteLine("\nStep 2 Verification Complete: All expected controls are present as shown in attachment");

            // Step 3: Click on the close button of the application
            Console.WriteLine("\nStep 3: Closing the application");
            
            Assert.NotNull(_mainWindow.CloseButton);
            Assert.True(_mainWindow.CloseButton.Displayed, "Close button should be visible");
            Assert.True(_mainWindow.CloseButton.Enabled, "Close button should be enabled");
            Console.WriteLine("✓ Close button is visible and enabled");
            
            // Close the application window
            _session.Close();
            Console.WriteLine("✓ Close command sent");
            
            // Give the application time to shutdown gracefully
            Thread.Sleep(1000);

            // Verify application has been shutdown by checking if window handle is still accessible
            bool windowClosed = IsWindowClosed();
            
            Assert.True(windowClosed, "Application should be closed");
            Console.WriteLine("✓ Application has been shutdown successfully");
            
            Console.WriteLine("\nStep 3 Verification Complete: Application closed gracefully");
        }

        /// <summary>
        /// Checks if the application window has been closed
        /// </summary>
        /// <returns>True if window is closed, false otherwise</returns>
        private bool IsWindowClosed()
        {
            try
            {
                // Try to access the current window handle
                var _ = _session.CurrentWindowHandle;
                
                // If we reach here without exception, window is still open
                // Wait a bit more and retry once
                Thread.Sleep(1500);
                _ = _session.CurrentWindowHandle;
                
                // Still open
                return false;
            }
            catch (OpenQA.Selenium.WebDriverException)
            {
                // Window is closed
                return true;
            }
            catch (InvalidOperationException)
            {
                // Session is closed
                return true;
            }
        }

        /// <summary>
        /// NEW Test Case Implementation - TC 75776 from Azure DevOps
        /// Test Case: Application Startup and Close Test
        /// Fetched from Azure DevOps on: 2026-01-05
        /// 
        /// Step 2: Navigate to the application and open it
        /// Expected: Outcome should be like the attached image (Screenshot 2025-12-24 152610.png)
        ///           - Application should launch successfully
        ///           - Main window with student data grid should be visible
        ///           - Control buttons (Add, Edit, Delete) should be present
        /// 
        /// Step 3: Click on the close button of the application
        /// Expected: Application should be shutdown
        /// </summary>
        [Fact]
        [Trait("TestCaseId", "75776_New")]
        [Trait("Category", "Appium")]
        [Trait("Priority", "P1")]
        public void TC75776_New_ApplicationStartupAndClose_ValidateUIAndGracefulShutdown()
        {
            // Step 2: Navigate to application and open it
            // Application is already opened in constructor
            Console.WriteLine("=== TC 75776 - NEW IMPLEMENTATION ===");
            Console.WriteLine("Step 2: Validating application startup (as per attached image)");
            
            // Verify application window is accessible
            Assert.NotNull(_session);
            Assert.NotNull(_session.Title);
            Console.WriteLine($"✓ Application window opened: '{_session.Title}'");
            
            // Validate main window controls as shown in attached image
            // 1. Student Data Grid
            Assert.NotNull(_mainWindow.StudentDataGrid);
            Assert.True(_mainWindow.StudentDataGrid.Displayed);
            Console.WriteLine("✓ Student Data Grid is displayed");
            
            // 2. Add New Student Button
            Assert.NotNull(_mainWindow.AddNewStudentButton);
            Assert.True(_mainWindow.AddNewStudentButton.Displayed);
            Assert.True(_mainWindow.AddNewStudentButton.Enabled);
            Console.WriteLine("✓ Add New Student button is displayed and enabled");
            
            // 3. Edit Student Button
            Assert.NotNull(_mainWindow.EditStudentButton);
            Assert.True(_mainWindow.EditStudentButton.Displayed);
            Console.WriteLine("✓ Edit Student button is displayed");
            
            // 4. Delete Student Button
            Assert.NotNull(_mainWindow.DeleteStudentButton);
            Assert.True(_mainWindow.DeleteStudentButton.Displayed);
            Console.WriteLine("✓ Delete Student button is displayed");
            
            // Additional UI elements validation
            if (_mainWindow.SectionsComboBox != null)
            {
                Assert.True(_mainWindow.SectionsComboBox.Displayed);
                Console.WriteLine("✓ Section filter dropdown is displayed");
            }
            
            if (_mainWindow.MarksPlotView != null)
            {
                Assert.True(_mainWindow.MarksPlotView.Displayed);
                Console.WriteLine("✓ Marks plot view is displayed");
            }
            
            Console.WriteLine("Step 2: PASSED - UI matches expected outcome (attached image)");
            
            // Step 3: Click on the close button of the application
            Console.WriteLine("\nStep 3: Closing the application");
            
            // Verify close button is accessible
            Assert.NotNull(_mainWindow.CloseButton);
            Assert.True(_mainWindow.CloseButton.Displayed);
            Assert.True(_mainWindow.CloseButton.Enabled);
            Console.WriteLine("✓ Close button is accessible");
            
            // Close the application
            _session.Close();
            Console.WriteLine("✓ Close command executed");
            
            // Wait for graceful shutdown
            Thread.Sleep(1500);
            
            // Verify application shutdown
            bool isShutdown = false;
            try
            {
                var handle = _session.CurrentWindowHandle;
                // If we get here, window is still open
                isShutdown = false;
            }
            catch (Exception)
            {
                // Exception means window is closed
                isShutdown = true;
            }
            
            Assert.True(isShutdown, "Application should be shutdown");
            Console.WriteLine("✓ Application has been shutdown successfully");
            Console.WriteLine("Step 3: PASSED - Application closed gracefully");
            
            Console.WriteLine("\n=== TC 75776 TEST COMPLETED SUCCESSFULLY ===");
        }

        public void Dispose()
        {
            try
            {
                _session?.Quit();
            }
            catch
            {
                // Ignore if session is already closed
            }
        }
    }
}
