namespace StudentDataViewerTests
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;
    using StudentDataViewer.ViewModel;

    public class AddStudentViewTests
    {
        private readonly WindowsDriver<WindowsElement> driver;

        public AddStudentViewTests()
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
            this.OpenAddStudentWindow();

            var addStudentDialogBox = this.driver.FindElementByAccessibilityId("AddStudentView");
            var closeButton = this.driver.FindElementByAccessibilityId("AddStudentCloseButton");
            closeButton.Click();
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("AddStudentView");
            });
        }

        [Fact]
        public void AddNewStudentWindowCloses_WhenCancelButton_IsClicked()
        {
            this.OpenAddStudentWindow();

            var addStudentDialogBox = this.driver.FindElementByAccessibilityId("AddStudentView");
            var CancelButton = this.driver.FindElementByAccessibilityId("AddStudentCancelButton");
            CancelButton.Click();
            Assert.Throws<WebDriverException>(() =>
            {
                this.driver.FindElementByAccessibilityId("AddStudentView");
            });
        }

        [Fact]
        public void AddNewStudent_ToList_WhenInputIsValid()
        {
            this.OpenAddStudentWindow();

            var addStudentDialogBox = this.driver.FindElementByAccessibilityId("AddStudentView");
            this.driver.FindElementByAccessibilityId("Name").SendKeys("Harith");
            var sectionComboBox = this.driver.FindElementByAccessibilityId("Section");
            sectionComboBox.SendKeys(OpenQA.Selenium.Keys.Alt + OpenQA.Selenium.Keys.ArrowDown);

            sectionComboBox.Click();

            var sections = sectionComboBox.FindElementsByClassName("ListBoxItem");
            if (sections.Count() != 0)
            {
                sections.First().Click();
            }
            else
            {
                sectionComboBox.SendKeys("D");
            }

            this.driver.FindElementByAccessibilityId("CGPA").SendKeys("9");

            this.driver.FindElementByAccessibilityId("AddStudentButton").Click();
            var messageBoxHandle = this.driver.WindowHandles.Last();
            this.driver.SwitchTo().Window(messageBoxHandle);
            var okButton = this.driver.FindElementByName("OK");
            okButton.Click();

            var sectionsComboBox = this.driver.FindElementByAccessibilityId("SectionsComboBox");
            sectionsComboBox.Click();

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.driver, TimeSpan.FromSeconds(5));
            var items = wait.Until(d => sectionsComboBox.FindElementsByClassName("ListBoxItem"));
            items.First().Click();

            var studentsGrid = this.driver.FindElementByAccessibilityId("StudentDataGrid");

            var rows = studentsGrid.FindElementsByClassName("DataGridRow");

            bool studentExists = false;

            foreach (var row in rows)
            {
                var cells = row.FindElementsByClassName("DataGridCell");

                foreach (var cell in cells)
                {
                    var textBlocks = cell.FindElementsByClassName("TextBlock");
                    foreach (var tb in textBlocks)
                    {
                        if (tb.Text.Contains("Harith"))
                        {
                            studentExists = true;
                            break;
                        }
                    }

                    if (studentExists)
                    {
                        break;
                    }
                }

                if (studentExists)
                {
                    break;
                }
            }

            Assert.True(studentExists);
        }

        private void OpenAddStudentWindow()
        {
            var addStudentButton = this.driver.FindElementByAccessibilityId("AddNewStudentButton");
            addStudentButton.Click();
        }
    }
}
