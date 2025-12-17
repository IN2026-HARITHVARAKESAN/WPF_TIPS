using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using StudentDataViewer;
using StudentDataViewer.Models;
using StudentDataViewer.ViewModel;
using StudentDataViewer.Views;

namespace StudentDataViewerTests
{
    public class MainViewTests
    {

        private readonly WindowsDriver<WindowsElement> driver;
        private MainViewModel mainViewModel;
        public MainViewTests()
        {
            var appOptions = new AppiumOptions();
            appOptions.AddAdditionalCapability("app", @"C:\Users\Harithvarakesan.bs\source\repos\wpf\WPF_TIPS\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe");
            appOptions.AddAdditionalCapability("deviceName", "WindowsPC");

            driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), appOptions);
            mainViewModel = new MainViewModel();
        }

        [Fact]
        public void SelectAll_ShouldDisplayAllStudents()
        {
            mainViewModel.SelectedSection = "ALL";
            Assert.NotEmpty(mainViewModel.StudentsToDisplay);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        public void SelectStudent_BySection(string section)
        {
            mainViewModel.SelectedSection = section;
            Assert.All(mainViewModel.StudentsToDisplay, student => Assert.Equal(section, student.Section));
        }

        [Fact]
        public void ButtonClicking_DeleteStudentButton_ShouldOpenDeleteDialogBox()
        {
            var deleteButton = driver.FindElementByAccessibilityId("DeleteStudentButton");
            deleteButton.Click();

            var deleteDialogBox = driver.FindElementByName("DeleteConfirmationView");
            
            Assert.NotNull(deleteDialogBox);
        }

        [Fact]
        public void ButtonClicking_EditStudentButton_ShouldOpenEditDialogBox()
        {
            //mainViewModel.SelectedStudent = mainViewModel.Students[0];
            var editStudentButton = driver.FindElementByAccessibilityId("EditStudentButton");
            editStudentButton.Click();

            var editStudentDialogBox = driver.FindElementByName("EditStudentView");

            //string studentId = editStudentDialogBox.FindElementByAccessibilityId("StudentId").Text;

            //Assert.Equal(mainViewModel.Students[0].StudentId, studentId);
            Assert.NotNull(editStudentDialogBox);
        }

        [Fact]
        public void ButtonClicking_AddStudentButton_ShouldOpenAddDialogBox()
        {
            var addStudentButton = driver.FindElementByAccessibilityId("AddNewStudentButton");
            addStudentButton.Click();

            var addStudentDialogBox = driver.FindElementByName("AddStudentView");
            Assert.NotNull(addStudentDialogBox);
        }
    }
}
