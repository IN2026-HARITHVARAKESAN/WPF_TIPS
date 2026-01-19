using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using StudentDataViewer.UITests.Configuration;

namespace StudentDataViewer.UITests.Infrastructure;

public class BaseTest : IDisposable
{
    protected WindowsDriver<WindowsElement> Driver { get; private set; }
    private bool _disposed;

    public BaseTest()
    {
        InitializeDriver();
    }

    private void InitializeDriver()
    {
        var appiumOptions = new AppiumOptions();
        appiumOptions.AddAdditionalCapability("app", TestConfig.ApplicationPath);
        appiumOptions.AddAdditionalCapability("platformName", "Windows");
        appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
        appiumOptions.AddAdditionalCapability("ms:waitForAppLaunch", "25");
        
        Driver = new WindowsDriver<WindowsElement>(new Uri(TestConfig.WinAppDriverUrl), appiumOptions);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TestConfig.ImplicitWaitSeconds);
    }

    protected void WaitForSeconds(int seconds)
    {
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            try
            {
                Driver?.CloseApp();
                Driver?.Quit();
                Driver?.Dispose();
            }
            catch (Exception)
            {
                // Ignore cleanup errors
            }
        }

        _disposed = true;
    }
}
