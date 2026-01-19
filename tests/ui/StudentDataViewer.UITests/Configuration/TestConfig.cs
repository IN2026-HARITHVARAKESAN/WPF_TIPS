namespace StudentDataViewer.UITests.Configuration;

public class TestConfig
{
    public const string WinAppDriverUrl = "http://127.0.0.1:4723";
    public const int ImplicitWaitSeconds = 10;
    public const int ExplicitWaitSeconds = 30;
    
    // Application settings
    public static string ApplicationPath => GetApplicationPath();
    
    private static string GetApplicationPath()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\..\..\src\StudentDataViewer\bin\Debug\net8.0-windows\StudentDataViewer.exe"));
        
        if (!File.Exists(projectDirectory))
        {
            throw new FileNotFoundException($"StudentDataViewer application not found at: {projectDirectory}. Please ensure the application is built.");
        }
        
        return projectDirectory;
    }
}
