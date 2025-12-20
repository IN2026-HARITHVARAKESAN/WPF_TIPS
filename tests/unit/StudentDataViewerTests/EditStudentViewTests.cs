namespace StudentDataViewerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    public class EditStudentViewTests
    {
        private readonly WindowsDriver<WindowsElement> driver;

        public EditStudentViewTests()
        {
            var appOptions = new AppiumOptions();
            appOptions.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appOptions.AddAdditionalCapability("deviceName", "WindowsPC");

            this.driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), appOptions);
        }

        [Fact]
        public void EditStudentWindowCloses_WhenCloseButton_IsClicked()
        {
            this.OpenEditStudentWindow();
            var closeButton = this.driver.FindElementByAccessibilityId("EditStudentCloseButton");
            closeButton.Click();
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("EditStudentView");
            });
        }

        [Fact]
        public void EditStudentWindowCloses_WhenCancelButton_IsClicked()
        {
            this.OpenEditStudentWindow();
            var cancelButton = this.driver.FindElementByAccessibilityId("CancelButton");
            cancelButton.Click();
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("EditStudentView");
            });
        }

        [Fact]
        public void ShouldEditStudent_WhenValidInputIsGiven()
        {
            this.OpenEditStudentWindow();

            var editStudentDialogBox = this.driver.FindElementByAccessibilityId("EditStudentView");
            var nameTxtBox = this.driver.FindElementByAccessibilityId("Name");
            nameTxtBox.Clear();
            nameTxtBox.SendKeys("Harith");

            var sectionComboBox = this.driver.FindElementByAccessibilityId("Section");
            sectionComboBox.Clear();
            sectionComboBox.SendKeys("A");

            var CgpaTxtBox = this.driver.FindElementByAccessibilityId("CGPA");
            CgpaTxtBox.Clear();
            CgpaTxtBox.SendKeys("9");

            var departmentComboBox = this.driver.FindElementByAccessibilityId("Department");
            departmentComboBox.Click();
            var departments = departmentComboBox.FindElementsByClassName("ListBoxItem");
            var itDept = departments.FirstOrDefault(item => item.FindElementByClassName("TextBlock").Text == "IT");

            var actions = new OpenQA.Selenium.Interactions.Actions(driver);
            actions.MoveToElement(itDept).Click().Perform();

            var yearComboBox = this.driver.FindElementByAccessibilityId("Year");
            yearComboBox.Click();
            var secondYear = yearComboBox.FindElementsByClassName("ListBoxItem").FirstOrDefault(item => item.Text == "2");
            actions = new OpenQA.Selenium.Interactions.Actions(driver);
            actions.MoveToElement(secondYear).Click().Perform();

            this.driver.FindElementByAccessibilityId("SaveButton").Click();

            var studentsGrid = this.driver.FindElementByAccessibilityId("StudentDataGrid").FindElementsByClassName("DataGridRow");

            bool studentExists = studentsGrid.Any(row =>
            {
                var cells = row.FindElementsByClassName("DataGridCell");
                return cells.Any(cell =>
                {
                    var textBlocks = cell.FindElementsByClassName("TextBlock");
                    return textBlocks.Any(tb => tb.Text.Contains("Harith"));
                });
            });

            Assert.True(studentExists);
        }

        private void OpenEditStudentWindow()
        {
            var sectionsComboBox = this.driver.FindElementByAccessibilityId("SectionsComboBox");
            sectionsComboBox.Click();

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.driver, TimeSpan.FromSeconds(5));
            var items = wait.Until(d => sectionsComboBox.FindElementsByClassName("ListBoxItem"));
            items.First().Click();

            var studentData = this.driver.FindElementsByAccessibilityId("StudentData").First();
            studentData.Click();

            var editStudentButton = this.driver.FindElementByAccessibilityId("EditStudentButton");
            editStudentButton.Click();
        }
    }
}
