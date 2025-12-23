using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;

public class MainWindowTests : IDisposable
{
    private readonly WindowsDriver<WindowsElement> _session;
    private readonly MainWindowPage _mainWindow;

    public MainWindowTests()
    {
        var appCapabilities = new AppiumOptions();
        appCapabilities.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
        appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
        _session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
        _mainWindow = new MainWindowPage(_session);
    }

    [Fact]
    [Trait("TestCaseId", "75776")]
    public void MainWindow_OpenAndClose_Test()
    {
        // 1. Verify main window has DataGrid and Add/Edit/Delete buttons
        Assert.NotNull(_mainWindow.StudentDataGrid);
        Assert.NotNull(_mainWindow.AddNewStudentButton);
        Assert.NotNull(_mainWindow.EditStudentButton);
        Assert.NotNull(_mainWindow.DeleteStudentButton);

        // 2. Click on the close button and verify app shuts down
        _mainWindow.Close();

        // Wait until accessing the UI throws (window closed)
        Assert.True(WaitUntil(() =>
            ThrowsWebDriver(() => _mainWindow.StudentDataGrid.Displayed)),
            "Expected application to be closed and elements to be inaccessible.");
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
                return true; // if access throws during evaluation, consider condition met
            }
            Thread.Sleep(pollMs);
        }
        return false;
    }

    public void Dispose()
    {
        try { _session?.Quit(); } catch { /* ignore if already closed */ }
    }
}