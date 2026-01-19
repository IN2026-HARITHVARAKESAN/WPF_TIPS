# StudentDataViewer UI Tests

This project contains automated UI tests for the StudentDataViewer WPF application using Appium WinAppDriver and XUnit.

## Test Case Coverage

### Test Case ID: 75860 - Delete Functionality Test

**Title:** [Appium Learning] Delete Functionality Test | StudentDataViewer Application Tests

**Description:** Comprehensive test suite for the delete functionality of the StudentDataViewer application.

## Prerequisites

1. **WinAppDriver** - Windows Application Driver must be installed and running
   - Download from: https://github.com/Microsoft/WinAppDriver/releases
   - Default URL: `http://127.0.0.1:4723`
   - Start WinAppDriver before running tests: `WinAppDriver.exe`

2. **StudentDataViewer Application** - The application must be built before running tests
   - Path: `src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe`
   - Build command: `dotnet build src\StudentDataViewer\StudentDataViewer.csproj`

## Project Structure

```
StudentDataViewer.UITests/
??? Configuration/
?   ??? TestConfig.cs                 # Test configuration and settings
??? Infrastructure/
?   ??? BaseTest.cs                   # Base test class with driver initialization
??? PageObjects/
?   ??? BasePage.cs                   # Base page object with common methods
?   ??? MainWindowPage.cs             # Main window page object
?   ??? DeleteStudentWindowPage.cs    # Delete confirmation window page object
??? Tests/
    ??? DeleteFunctionalityTests.cs   # Delete functionality test cases
```

## Page Objects

### MainWindowPage
Represents the main window of the StudentDataViewer application with methods to:
- Select sections from dropdown
- Interact with student data grid
- Select/deselect student checkboxes
- Click action buttons (Add, Edit, Delete)
- Verify window state

### DeleteStudentWindowPage
Represents the delete confirmation dialog with methods to:
- Verify window display
- Check selected students list
- Confirm or cancel deletion
- Verify window closure

## Test Cases

### 1. DeleteFunctionality_CompleteWorkflow_ShouldWorkCorrectly
**Test Case ID:** 75860

Complete workflow test that validates:
1. Application launch and main window display
2. Opening delete window with no selection
3. Canceling deletion
4. Selecting all students
5. Deleting single student
6. Deleting all remaining students

### 2. DeleteFunctionality_CancelDeletion_ShouldNotDeleteStudent
**Test Case ID:** 75860

Validates that:
- Selecting a student and canceling deletion doesn't remove the student
- Student count remains unchanged after cancellation

### 3. DeleteFunctionality_DeleteSingleStudent_ShouldRemoveOnlySelectedStudent
**Test Case ID:** 75860

Validates that:
- Only the selected student is removed
- Student count decreases by exactly 1

## Running the Tests

### Using Visual Studio
1. Start WinAppDriver: `WinAppDriver.exe`
2. Build the solution
3. Open Test Explorer
4. Run all tests or individual tests

### Using Command Line
```powershell
# Start WinAppDriver in a separate terminal
WinAppDriver.exe

# Build the solution
dotnet build

# Run all tests
dotnet test tests/ui/StudentDataViewer.UITests/StudentDataViewer.UITests.csproj

# Run tests with specific trait
dotnet test --filter "TestCaseId=75860"

# Run tests with category
dotnet test --filter "Category=UI"
```

## Configuration

### TestConfig.cs Settings
- **WinAppDriverUrl**: `http://127.0.0.1:4723`
- **ImplicitWaitSeconds**: 10 seconds
- **ExplicitWaitSeconds**: 30 seconds
- **ApplicationPath**: Auto-detected from build output

### Modifying Application Path
If the application is built to a different location, update the `GetApplicationPath()` method in `TestConfig.cs`.

## Troubleshooting

### WinAppDriver Connection Issues
- Ensure WinAppDriver is running before executing tests
- Check if port 4723 is available
- Verify firewall settings

### Element Not Found Errors
- Ensure the StudentDataViewer application is built with the correct AutomationId properties
- Verify element locators match the XAML definitions
- Increase wait times if needed

### Application Launch Issues
- Verify the application path in TestConfig.cs
- Ensure the application is built for Debug configuration
- Check if .NET 8 runtime is installed

## Dependencies

- **Appium.WebDriver** (5.0.0-rc.1) - Appium .NET client
- **xUnit** (2.9.2) - Testing framework
- **xUnit.runner.visualstudio** (2.8.2) - Visual Studio test adapter
- **Microsoft.NET.Test.Sdk** (17.11.1) - Test SDK
- **Castle.Core** (5.1.1) - Required for Appium

## Best Practices

1. **Page Object Pattern**: All UI interactions are encapsulated in page objects
2. **Wait Strategies**: Explicit waits are used for better test stability
3. **Resource Cleanup**: BaseTest implements IDisposable for proper cleanup
4. **Test Independence**: Each test can run independently
5. **Descriptive Names**: Test methods have clear, descriptive names

## Future Enhancements

- Add tests for Add Student functionality
- Add tests for Edit Student functionality
- Implement parallel test execution
- Add screenshot capture on test failure
- Implement test data management
- Add performance metrics

## Contributing

When adding new tests:
1. Create page objects for new windows/dialogs
2. Follow existing naming conventions
3. Use explicit waits instead of Thread.Sleep where possible
4. Add appropriate test traits (Category, TestCaseId)
5. Document complex test scenarios

## License

This test project is part of the StudentDataViewer application.
