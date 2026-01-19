using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using StudentDataViewer.UITests.Configuration;

namespace StudentDataViewer.UITests.PageObjects;

public abstract class BasePage
{
    protected readonly WindowsDriver<WindowsElement> Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(WindowsDriver<WindowsElement> driver)
    {
        Driver = driver ?? throw new ArgumentNullException(nameof(driver));
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(TestConfig.ExplicitWaitSeconds));
    }

    protected WindowsElement FindElement(By locator)
    {
        return Driver.FindElement(locator) as WindowsElement 
            ?? throw new NoSuchElementException($"Element not found: {locator}");
    }

    protected IReadOnlyCollection<WindowsElement> FindElements(By locator)
    {
        return Driver.FindElements(locator).Cast<WindowsElement>().ToList();
    }

    protected void Click(By locator)
    {
        WaitForElementToBeClickable(locator);
        FindElement(locator).Click();
    }

    protected void SendKeys(By locator, string text)
    {
        WaitForElementToBeVisible(locator);
        var element = FindElement(locator);
        element.Clear();
        element.SendKeys(text);
    }

    protected string GetText(By locator)
    {
        WaitForElementToBeVisible(locator);
        return FindElement(locator).Text;
    }

    protected bool IsElementDisplayed(By locator)
    {
        try
        {
            return FindElement(locator).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    protected void WaitForElementToBeVisible(By locator)
    {
        Wait.Until(driver => 
        {
            try
            {
                var element = driver.FindElement(locator);
                return element != null && element.Displayed;
            }
            catch
            {
                return false;
            }
        });
    }

    protected void WaitForElementToBeClickable(By locator)
    {
        Wait.Until(driver => 
        {
            try
            {
                var element = driver.FindElement(locator);
                return element != null && element.Displayed && element.Enabled;
            }
            catch
            {
                return false;
            }
        });
    }

    protected void WaitForWindowToLoad(string windowName, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(driver =>
        {
            try
            {
                var window = driver.FindElement(By.Name(windowName));
                return window != null;
            }
            catch
            {
                return false;
            }
        });
    }

    protected static By ByAccessibilityId(string accessibilityId)
    {
        return By.XPath($"//*[@AutomationId='{accessibilityId}']");
    }
}
