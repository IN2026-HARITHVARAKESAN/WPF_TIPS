using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace StudentDataViewerTests
{
    public class DeleteStudentViewTest
    {
        private readonly WindowsDriver<WindowsElement> driver;

        public DeleteStudentViewTest()
        {
            var appOptions = new AppiumOptions();
            appOptions.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appOptions.AddAdditionalCapability("deviceName", "WindowsPC");

            this.driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), appOptions);
        }

        [Fact]
        public void AddNewStudentWindowCloses_WhenCloseButton_IsClicked()
        {
            this.OpenDeleteStudentWindow();
            var closeButton = this.driver.FindElementByName("Close");
            closeButton.Click();
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("DeleteStudentView");
            });
        }

        [Fact]
        public void ShouldDelete_SelectedStudent_WhenDeleteButtonClicked()
        {
            this.OpenDeleteStudentWindow();
            var studentsBeforeDelete = this.driver.FindElementByAccessibilityId("StudentDataGrid").FindElementsByClassName("DataGridRow").Count();

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.driver, TimeSpan.FromSeconds(5));
            var studentsToDelete = wait.Until(d => this.driver.FindElementByAccessibilityId("SelectedStudentsList").FindElementsByClassName("ListViewItem"));
            var yesButton = this.driver.FindElementByAccessibilityId("ConfirmationYes");
            yesButton.Click();

            var studentsAfterDelete = this.driver.FindElementByAccessibilityId("StudentDataGrid").FindElementsByClassName("DataGridRow").Count();
            Assert.Equal(studentsBeforeDelete - studentsToDelete.Count(), studentsAfterDelete);
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("DeleteStudentView");
            });
        }

        [Fact]
        public void ShouldNotDelete_SelectedStudent_WhenNoButtonClicked()
        {
            this.OpenDeleteStudentWindow();
            var studentsBeforeDelete = this.driver.FindElementByAccessibilityId("StudentDataGrid").FindElementsByClassName("DataGridRow").Count();

            var noButton = this.driver.FindElementByAccessibilityId("ConfirmationNo");
            noButton.Click();

            var studentsAfterDelete = this.driver.FindElementByAccessibilityId("StudentDataGrid").FindElementsByClassName("DataGridRow").Count();
            Assert.Equal(studentsBeforeDelete, studentsAfterDelete);
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("DeleteStudentView");
            });
        }

        private void OpenDeleteStudentWindow()
        {
            var sectionsComboBox = this.driver.FindElementByAccessibilityId("SectionsComboBox");
            sectionsComboBox.Click();

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.driver, TimeSpan.FromSeconds(5));
            var items = wait.Until(d => sectionsComboBox.FindElementsByClassName("ListBoxItem"));
            items.First().Click();

            var selectAllCheckBox = this.driver.FindElementByAccessibilityId("SelectAllCheckBox");
            selectAllCheckBox.Click();

            var addStudentButton = this.driver.FindElementByAccessibilityId("DeleteStudentButton");
            addStudentButton.Click();
        }
    }
}
