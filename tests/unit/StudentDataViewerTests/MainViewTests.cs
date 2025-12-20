namespace StudentDataViewerTests
{
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;
    using StudentDataViewer.ViewModel;

    public class MainViewTests
    {
        private readonly WindowsDriver<WindowsElement> driver;
        private MainViewModel mainViewModel;

        public MainViewTests()
        {
            var appOptions = new AppiumOptions();
            appOptions.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appOptions.AddAdditionalCapability("deviceName", "WindowsPC");

            this.driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), appOptions);
            this.mainViewModel = new MainViewModel();
        }

        [Fact]
        public void SelectAll_ShouldDisplayAllStudents()
        {
            this.mainViewModel.SelectedSection = "ALL";
            Assert.NotEmpty(this.mainViewModel.StudentsToDisplay);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        public void SelectStudent_BySection(string section)
        {
            this.mainViewModel.SelectedSection = section;
            Assert.All(this.mainViewModel.StudentsToDisplay, student => Assert.Equal(section, student.Section));
        }

        [Fact]
        public void SelectStudents_UsingCheckbox()
        {
            this.mainViewModel.SelectedSection = "A";
            var selectAllCheckBox = this.driver.FindElementByAccessibilityId("SelectAllCheckBox");
            selectAllCheckBox.Click();

            Assert.All(this.mainViewModel.Students.Where(student => student.Section == "A"), student => Assert.Equal("A", student.Section));
            Assert.All(this.mainViewModel.Students.Where(student => student.Section != "A"), student => Assert.NotEqual("A", student.Section));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("C")]
        public void AddNewSection_ShouldAddOnlyUniqueSection(string section)
        {
            this.mainViewModel.NewSection = section;
            this.mainViewModel.AddNewSection();

            Assert.Single(this.mainViewModel.Sections.Where(sec => sec == section));
        }

        [Fact]
        public void ButtonClicking_DeleteStudentButton_ShouldOpenDeleteDialogBox()
        {
            var deleteButton = this.driver.FindElementByAccessibilityId("DeleteStudentButton");
            deleteButton.Click();

            var deleteDialogBox = this.driver.FindElementByAccessibilityId("DeleteConfirmationView");
            Assert.NotNull(deleteDialogBox);
        }

        [Fact]
        public void ButtonClicking_EditStudentButton_ShouldOpenEditDialogBox()
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

            var editStudentDialogBox = this.driver.FindElementByAccessibilityId("EditStudentView");

            string studentId = editStudentDialogBox.FindElementByAccessibilityId("StudentId").Text;

            Assert.Equal(this.mainViewModel.Students[0].StudentId, studentId);
            Assert.NotNull(editStudentDialogBox);
        }

        [Fact]
        public void ButtonClicking_AddStudentButton_ShouldOpenAddDialogBox()
        {
            var addStudentButton = this.driver.FindElementByAccessibilityId("AddNewStudentButton");
            addStudentButton.Click();

            var addStudentDialogBox = this.driver.FindElementByAccessibilityId("AddStudentView");
            Assert.NotNull(addStudentDialogBox);
        }
    }
}
