# Azure DevOps Pipeline Setup - StudentDataViewer Project

This document provides **project-specific** instructions for setting up the Azure DevOps pipeline for the **StudentDataViewer** WPF application with WinAppDriver automated UI tests.

## Table of Contents
- [Project Overview](#project-overview)
- [Pipeline Configuration](#pipeline-configuration)
- [Step-by-Step Setup](#step-by-step-setup)
- [Pipeline Workflow Explained](#pipeline-workflow-explained)
- [Testing the Pipeline](#testing-the-pipeline)
- [Troubleshooting](#troubleshooting)

---

## Project Overview

### Project Structure
```
WPF_TIPS/
??? src/
?   ??? StudentDataViewer/              # Main WPF application
?       ??? StudentDataViewer.csproj
?       ??? App.xaml
?       ??? ... (other source files)
??? tests/
?   ??? unit/
?       ??? StudentDataViewerTest/      # WinAppDriver UI tests
?           ??? StudentDataViewerTest.csproj
?           ??? ApplicationStartupAndCloseTests.cs
?           ??? AddStudentFunctionalityTests.cs
?           ??? EditFunctionalityTests.cs
?           ??? DeleteFunctionalityTests.cs
?           ??? ... (other test files)
??? azure-pipelines.yml                 # Pipeline configuration
??? run-ui-tests.ps1                    # Local test runner script
```

### Technology Stack
- **Application**: WPF (.NET Framework 4.8)
- **Tests**: .NET 8, Appium.WebDriver, MSTest
- **Automation**: WinAppDriver 1.2.1
- **CI/CD**: Azure DevOps Pipelines

### Git Repository
- **Branch**: `wpf_training_TestCaseAutomation`
- **Remote**: `https://github.com/IN2026-HARITHVARAKESAN/WPF_TIPS`

---

## Pipeline Configuration

The `azure-pipelines.yml` file in your repository root contains the complete pipeline configuration.

### Trigger Configuration

#### Automatic Triggers (Push to Branches)
```yaml
trigger:
  branches:
    include:
      - main
      - wpf_training_TestCaseAutomation
  paths:
    include:
      - src/StudentDataViewer/**
      - tests/unit/StudentDataViewerTest/**
```

**What This Means**:
- Pipeline runs automatically when code is pushed to `main` or `wpf_training_TestCaseAutomation` branches
- Only triggers if changes are in the `StudentDataViewer` source code or test project
- Changes to other files (like documentation) won't trigger the pipeline

#### Pull Request Triggers
```yaml
pr:
  branches:
    include:
      - main
  paths:
    include:
      - src/StudentDataViewer/**
      - tests/unit/StudentDataViewerTest/**
```

**What This Means**:
- Pipeline runs when a Pull Request targets the `main` branch
- Only runs if the PR includes changes to the application or tests
- Helps validate code before merging

### Agent Configuration
```yaml
pool:
  vmImage: 'windows-latest'
```

**What This Means**:
- Azure DevOps provisions a Windows Server VM (latest version)
- WinAppDriver requires Windows, so this is mandatory
- VM is destroyed after the pipeline completes

### Variables
```yaml
variables:
  buildConfiguration: 'Release'
  solution: '**/*.sln'
  testProject: 'tests/unit/StudentDataViewerTest/StudentDataViewerTest.csproj'
```

**What This Means**:
- `buildConfiguration`: Builds in Release mode (optimized)
- `solution`: Locates any `.sln` file in the repository
- `testProject`: Points to the StudentDataViewerTest project

---

## Step-by-Step Setup

### Step 1: Verify Repository Structure

Ensure your repository has the correct structure:
```bash
git clone https://github.com/IN2026-HARITHVARAKESAN/WPF_TIPS.git
cd WPF_TIPS
git checkout wpf_training_TestCaseAutomation
```

Verify these files exist:
- `src/StudentDataViewer/StudentDataViewer.csproj`
- `tests/unit/StudentDataViewerTest/StudentDataViewerTest.csproj`
- `azure-pipelines.yml` (in the root)

### Step 2: Review the Pipeline File

Open `azure-pipelines.yml` and verify it matches your project structure. Key sections:

#### ? Build Configuration
```yaml
- task: PowerShell@2
  displayName: 'Build StudentDataViewer WPF Application'
  inputs:
    targetType: 'inline'
    script: |
      dotnet build src/StudentDataViewer/StudentDataViewer.csproj `
        --configuration $(buildConfiguration) `
        --output $(Build.SourcesDirectory)/build/StudentDataViewer
      Write-Host "Build completed"
```

This step:
1. Builds the `StudentDataViewer.csproj` project
2. Outputs the compiled EXE to `build/StudentDataViewer/`
3. Uses the Release configuration for optimized binaries

#### ? EXE Verification
```yaml
- task: PowerShell@2
  displayName: 'Verify EXE file exists'
  inputs:
    targetType: 'inline'
    script: |
      $exePath = "$(Build.SourcesDirectory)/build/StudentDataViewer/StudentDataViewer.exe"
      if (Test-Path $exePath) {
        Write-Host "##[section]? EXE file found at: $exePath"
        Get-Item $exePath | Format-List
      } else {
        Write-Host "##[error]? EXE file not found!"
        Write-Host "Directory contents:"
        Get-ChildItem "$(Build.SourcesDirectory)/build/StudentDataViewer" -Recurse
        exit 1
      }
```

This step:
1. Checks if `StudentDataViewer.exe` was created successfully
2. Lists the file properties if found
3. Fails the pipeline if the EXE is missing

#### ? WinAppDriver Installation
```yaml
- task: PowerShell@2
  displayName: 'Download and Install WinAppDriver'
  inputs:
    targetType: 'inline'
    script: |
      $winAppDriverUrl = "https://github.com/microsoft/WinAppDriver/releases/download/v1.2.1/WindowsApplicationDriver.msi"
      $installerPath = "$env:TEMP\WinAppDriver.msi"
      
      Write-Host "Downloading WinAppDriver..."
      Invoke-WebRequest -Uri $winAppDriverUrl -OutFile $installerPath
      
      Write-Host "Installing WinAppDriver..."
      Start-Process msiexec.exe -ArgumentList "/i", $installerPath, "/quiet", "/norestart" -Wait
      
      Write-Host "##[section]WinAppDriver installed successfully"
```

This step:
1. Downloads WinAppDriver v1.2.1 from GitHub
2. Installs it silently (no user interaction needed)
3. Verifies installation completed

#### ? WinAppDriver Startup
```yaml
- task: PowerShell@2
  displayName: 'Start WinAppDriver'
  inputs:
    targetType: 'inline'
    script: |
      $winAppDriverPath = "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
      
      if (Test-Path $winAppDriverPath) {
        Write-Host "Starting WinAppDriver at http://127.0.0.1:4723"
        Start-Process -FilePath $winAppDriverPath -WindowStyle Hidden
        Start-Sleep -Seconds 5
        
        # Verify WinAppDriver is running
        $process = Get-Process -Name "WinAppDriver" -ErrorAction SilentlyContinue
        if ($process) {
          Write-Host "##[section]? WinAppDriver is running (PID: $($process.Id))"
        } else {
          Write-Host "##[error]? WinAppDriver failed to start"
          exit 1
        }
      } else {
        Write-Host "##[error]? WinAppDriver.exe not found at expected path"
        exit 1
      }
```

This step:
1. Locates the WinAppDriver executable
2. Starts it in the background on port 4723
3. Waits 5 seconds for it to initialize
4. Verifies the process is running

#### ? Test Configuration
```yaml
- task: PowerShell@2
  displayName: 'Update Test Configuration'
  inputs:
    targetType: 'inline'
    script: |
      $appPath = "$(Build.SourcesDirectory)/build/StudentDataViewer/StudentDataViewer.exe"
      
      # Create test settings
      $testSettings = @{
        AppPath = $appPath
      } | ConvertTo-Json
      
      $testSettings | Out-File -FilePath "tests/unit/StudentDataViewerTest/testsettings.json" -Encoding UTF8
      
      Write-Host "Test settings created with AppPath: $appPath"
```

This step:
1. Creates a `testsettings.json` file with the path to the EXE
2. Your tests can read this file to know where the application is located
3. Ensures tests run against the built executable

#### ? Run Tests
```yaml
- task: PowerShell@2
  displayName: 'Run WinAppDriver Tests'
  inputs:
    targetType: 'inline'
    script: |
      $env:TEST_APP_PATH = "$(Build.SourcesDirectory)/build/StudentDataViewer/StudentDataViewer.exe"
      Write-Host "Running UI tests with app path: $env:TEST_APP_PATH"
      
      dotnet test $(testProject) `
        --configuration $(buildConfiguration) `
        --no-build `
        --logger "trx;LogFileName=test-results.trx" `
        --logger "console;verbosity=detailed"
  continueOnError: true
```

This step:
1. Sets the `TEST_APP_PATH` environment variable (tests can read this)
2. Runs all tests in the `StudentDataViewerTest` project
3. Generates a `.trx` file with test results
4. Uses detailed console logging for debugging
5. Continues even if tests fail (so results can be published)

#### ? Cleanup
```yaml
- task: PowerShell@2
  displayName: 'Stop WinAppDriver'
  condition: always()
  inputs:
    targetType: 'inline'
    script: |
      $process = Get-Process -Name "WinAppDriver" -ErrorAction SilentlyContinue
      if ($process) {
        Stop-Process -Name "WinAppDriver" -Force
        Write-Host "WinAppDriver stopped"
      }
```

This step:
1. Runs even if previous steps fail (`condition: always()`)
2. Gracefully stops WinAppDriver
3. Prevents resource leaks on the agent VM

#### ? Publish Results
```yaml
- task: PublishTestResults@2
  displayName: 'Publish Test Results'
  condition: always()
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/test-results.trx'
    mergeTestResults: true
    failTaskOnFailedTests: true
    testRunTitle: 'WinAppDriver UI Tests'
```

This step:
1. Uploads test results to Azure DevOps
2. Makes results visible in the "Tests" tab
3. Fails the pipeline if any tests failed
4. Provides detailed test analytics and history

#### ? Artifacts (On Failure)
```yaml
- task: PublishPipelineArtifact@1
  displayName: 'Upload Test Artifacts'
  condition: failed()
  inputs:
    targetPath: '$(Build.SourcesDirectory)/tests/unit/StudentDataViewerTest/TestResults'
    artifact: 'test-results'
    publishLocation: 'pipeline'
```

This step:
1. Runs only if the pipeline fails (`condition: failed()`)
2. Uploads the `TestResults` folder as an artifact
3. Useful for debugging test failures (screenshots, logs, etc.)

### Step 3: Commit the Pipeline File (If Modified)

If you made changes to `azure-pipelines.yml`:

```bash
git add azure-pipelines.yml
git commit -m "Update Azure DevOps pipeline configuration"
git push origin wpf_training_TestCaseAutomation
```

### Step 4: Create the Pipeline in Azure DevOps

1. **Navigate to Azure DevOps**:
   - Go to: `https://dev.azure.com/<your-organization>/<your-project>`

2. **Create a New Pipeline**:
   - Click **Pipelines** ? **New Pipeline**
   - Select **GitHub** (or **Azure Repos Git** if hosted there)
   - Authenticate and select your repository: `IN2026-HARITHVARAKESAN/WPF_TIPS`

3. **Configure the Pipeline**:
   - Select **Existing Azure Pipelines YAML file**
   - Branch: `wpf_training_TestCaseAutomation`
   - Path: `/azure-pipelines.yml`
   - Click **Continue**

4. **Review and Run**:
   - Review the YAML configuration
   - Click **Run** to execute the pipeline for the first time

### Step 5: Monitor the Pipeline Execution

Once the pipeline starts:

1. **View Live Logs**:
   - Click on the running pipeline
   - Click on the **WinAppDriverTests** job
   - Watch each step execute in real-time

2. **Check for Errors**:
   - If any step fails, click on it to view detailed logs
   - Common issues are listed in the [Troubleshooting](#troubleshooting) section

3. **Review Test Results**:
   - After completion, go to the **Tests** tab
   - View pass/fail status for each test
   - Drill down into failed tests for details

---

## Pipeline Workflow Explained

### Complete Workflow Diagram

```
???????????????????????????????????????????????????????
? 1. Trigger (Push or PR)                             ?
?    ??? Branch: main or wpf_training_TestCaseAutomation ?
?    ??? Paths: src/StudentDataViewer/** or tests/** ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 2. Provision Windows VM (windows-latest)            ?
?    ??? Azure DevOps creates a fresh Windows Server  ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 3. Checkout Code                                     ?
?    ??? Clone wpf_training_TestCaseAutomation branch ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 4. Setup .NET 8 SDK                                  ?
?    ??? Install .NET 8.0.x for building and testing  ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 5. Restore Dependencies                              ?
?    ??? Restore NuGet packages for the solution      ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 6. Build StudentDataViewer Application              ?
?    ??? Compile src/StudentDataViewer.csproj         ?
?    ??? Configuration: Release                       ?
?    ??? Output: build/StudentDataViewer/*.exe        ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 7. Verify EXE Exists                                 ?
?    ??? Check: StudentDataViewer.exe                 ?
?    ??? If missing ? FAIL pipeline                   ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 8. Download & Install WinAppDriver v1.2.1           ?
?    ??? Download from GitHub releases                ?
?    ??? Install silently via msiexec                 ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 9. Start WinAppDriver Server                        ?
?    ??? Launch on http://127.0.0.1:4723              ?
?    ??? Wait 5 seconds for initialization            ?
?    ??? Verify process is running                    ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 10. Create Test Configuration                        ?
?     ??? Generate testsettings.json with EXE path    ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 11. Run WinAppDriver UI Tests                       ?
?     ??? Execute StudentDataViewerTest project       ?
?     ??? Tests: Add, Edit, Delete, Startup, etc.    ?
?     ??? Generate test-results.trx                   ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 12. Stop WinAppDriver (Always runs)                 ?
?     ??? Cleanup: Kill WinAppDriver process          ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 13. Publish Test Results (Always runs)              ?
?     ??? Upload .trx file to Azure DevOps           ?
?     ??? Display in Tests tab                        ?
?     ??? If tests failed ? FAIL pipeline             ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 14. Upload Artifacts (If pipeline failed)           ?
?     ??? Upload TestResults folder for debugging     ?
???????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????
? 15. Pipeline Complete                                ?
?     ??? Status: SUCCESS or FAILURE                  ?
?     ??? Destroy Windows VM                          ?
???????????????????????????????????????????????????????
```

### Test Execution Flow

When tests run, here's what happens:

1. **Test Discovery**:
   - MSTest discovers all test classes in `StudentDataViewerTest`:
     - `ApplicationStartupAndCloseTests`
     - `AddStudentFunctionalityTests`
     - `EditFunctionalityTests`
     - `DeleteFunctionalityTests`
     - `DeleteConfirmationPageTests`
     - `MainWindowTests`
     - `AddStudentPageTests`
     - `EditStudentPageTests`

2. **Test Initialization** (OneTimeSetUp):
   - Read `TEST_APP_PATH` environment variable
   - Start WinAppDriver session
   - Launch `StudentDataViewer.exe`
   - Wait for main window to be ready

3. **Test Execution**:
   - Each `[TestMethod]` runs sequentially
   - Tests interact with the WPF application via WinAppDriver
   - UI elements are located by AutomationId, Name, or XPath
   - Actions are performed (click, type, select, etc.)
   - Assertions verify expected behavior

4. **Test Cleanup** (OneTimeTearDown):
   - Close the application
   - Quit WinAppDriver session
   - Clean up resources

5. **Result Reporting**:
   - Generate `test-results.trx` file
   - Upload to Azure DevOps
   - Display in Tests tab with pass/fail status

---

## Testing the Pipeline

### Local Testing Before Pushing

Before relying on Azure DevOps, test locally using the `run-ui-tests.ps1` script:

```powershell
# From the repository root
.\run-ui-tests.ps1
```

This script:
1. Builds the StudentDataViewer application
2. Starts WinAppDriver (if not running)
3. Runs the tests
4. Displays results

**Benefits**:
- Faster feedback (no waiting for CI/CD)
- Easier debugging (attach debugger to tests)
- Validates changes before pushing

### Triggering the Pipeline

#### Method 1: Push to Watched Branches
```bash
git checkout wpf_training_TestCaseAutomation
git add .
git commit -m "Fix: Update student validation logic"
git push origin wpf_training_TestCaseAutomation
```

The pipeline runs automatically if changes are in:
- `src/StudentDataViewer/**`
- `tests/unit/StudentDataViewerTest/**`

#### Method 2: Create a Pull Request
```bash
git checkout -b feature/new-student-form
# Make changes...
git add .
git commit -m "Feature: Add new student form"
git push origin feature/new-student-form
```

Then create a PR targeting `main` ? pipeline runs automatically.

#### Method 3: Manual Trigger
1. Go to Azure DevOps ? Pipelines
2. Select your pipeline
3. Click **Run pipeline**
4. Choose branch: `wpf_training_TestCaseAutomation`
5. Click **Run**

### Viewing Results

#### Tests Tab
1. Go to the pipeline run
2. Click the **Tests** tab
3. View:
   - Total tests: `X passed, Y failed`
   - Test duration
   - Failure details (stack trace, error message)

#### Logs Tab
1. Go to the pipeline run
2. Click the **Logs** tab
3. Expand each step to see detailed output

#### Artifacts (On Failure)
1. Go to the failed pipeline run
2. Click the **Artifacts** tab
3. Download `test-results` artifact
4. Extract and review:
   - Screenshots (if captured by tests)
   - Log files
   - Test result files

---

## Troubleshooting

### Common Issues and Solutions

#### 1. **Pipeline Not Triggering**

**Symptom**: Push code but pipeline doesn't run.

**Causes**:
- Changes not in watched paths (`src/StudentDataViewer/**` or `tests/**`)
- Branch not in trigger list

**Solution**:
1. Check `azure-pipelines.yml`:
   ```yaml
   trigger:
     branches:
       include:
         - wpf_training_TestCaseAutomation  # Your branch here
   ```
2. Push a change to a watched file:
   ```bash
   # Dummy change to trigger pipeline
   touch src/StudentDataViewer/App.xaml
   git add .
   git commit -m "Trigger pipeline"
   git push
   ```

#### 2. **Build Fails: "Project not found"**

**Symptom**: Build step fails with "Could not find project or directory".

**Causes**:
- Incorrect path to `.csproj` file
- File moved or renamed

**Solution**:
1. Verify project path:
   ```bash
   ls src/StudentDataViewer/StudentDataViewer.csproj
   ```
2. Update pipeline if needed:
   ```yaml
   script: |
     dotnet build src/StudentDataViewer/StudentDataViewer.csproj
   ```

#### 3. **EXE Not Found After Build**

**Symptom**: "Verify EXE file exists" step fails.

**Causes**:
- Build output path mismatch
- Build failed silently

**Solution**:
1. Check build output directory:
   ```yaml
   script: |
     dotnet build src/StudentDataViewer/StudentDataViewer.csproj --output $(Build.SourcesDirectory)/build/StudentDataViewer
     ls $(Build.SourcesDirectory)/build/StudentDataViewer
   ```
2. Verify the EXE name matches your project name

#### 4. **WinAppDriver Fails to Start**

**Symptom**: "WinAppDriver failed to start" error.

**Causes**:
- Installation failed
- Port 4723 already in use
- Insufficient wait time

**Solution**:
1. Increase wait time:
   ```yaml
   Start-Sleep -Seconds 10  # Increase from 5 to 10
   ```
2. Add port check:
   ```powershell
   netstat -an | findstr "4723"
   ```

#### 5. **Tests Fail: "Could not find element"**

**Symptom**: Tests fail with "An element could not be located".

**Causes**:
- UI not fully loaded
- AutomationId changed
- Application window not in focus

**Solution**:
1. Add implicit waits in test code:
   ```csharp
   session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
   ```
2. Verify AutomationIds in XAML:
   ```xml
   <Button x:Name="AddButton" AutomationProperties.AutomationId="AddButton" />
   ```
3. Add explicit waits before interactions:
   ```csharp
   var wait = new WebDriverWait(session, TimeSpan.FromSeconds(10));
   wait.Until(d => d.FindElementByAccessibilityId("AddButton"));
   ```

#### 6. **Tests Pass Locally But Fail in Pipeline**

**Symptom**: Tests succeed on your machine but fail in Azure DevOps.

**Causes**:
- Environment differences (screen resolution, DPI settings)
- Timing issues (VM slower than local machine)
- Missing dependencies

**Solution**:
1. Add more verbose logging in tests:
   ```csharp
   Console.WriteLine($"Current window: {session.Title}");
   Console.WriteLine($"Available elements: {string.Join(", ", session.FindElementsByXPath("//*").Select(e => e.GetAttribute("Name")))}");
   ```
2. Increase timeouts:
   ```csharp
   Thread.Sleep(2000);  // Wait for UI to stabilize
   ```
3. Capture screenshots on failure:
   ```csharp
   if (testFailed) {
       var screenshot = session.GetScreenshot();
       screenshot.SaveAsFile($"failure_{testName}.png");
   }
   ```

#### 7. **Pipeline Takes Too Long**

**Symptom**: Pipeline runs for >10 minutes.

**Causes**:
- Slow VM provisioning
- Downloading large dependencies
- Tests running slowly

**Solution**:
1. Cache NuGet packages (add to pipeline):
   ```yaml
   - task: Cache@2
     inputs:
       key: 'nuget | "$(Agent.OS)" | **/packages.lock.json'
       path: $(NUGET_PACKAGES)
   ```
2. Optimize test execution:
   - Run tests in parallel (if independent)
   - Reduce unnecessary waits

#### 8. **Test Results Not Published**

**Symptom**: "Tests" tab shows no results.

**Causes**:
- `.trx` file not generated
- Path mismatch in PublishTestResults task

**Solution**:
1. Verify `.trx` file is created:
   ```yaml
   script: |
     dotnet test --logger "trx;LogFileName=test-results.trx"
     ls tests/unit/StudentDataViewerTest/TestResults
   ```
2. Check `testResultsFiles` path:
   ```yaml
   testResultsFiles: '**/test-results.trx'
   ```

---

## Additional Tips

### Debugging Failed Tests

1. **Download Artifacts**:
   - Go to the failed run ? Artifacts tab
   - Download `test-results` folder
   - Review screenshots and logs

2. **Enable Detailed Logging**:
   ```yaml
   --logger "console;verbosity=detailed"
   ```

3. **Add Diagnostic Output in Tests**:
   ```csharp
   [TestMethod]
   public void TestAddStudent()
   {
       Console.WriteLine("Starting TestAddStudent");
       Console.WriteLine($"Window title: {session.Title}");
       
       var addButton = session.FindElementByAccessibilityId("AddButton");
       Console.WriteLine($"Found AddButton: {addButton.Displayed}");
       
       addButton.Click();
       Console.WriteLine("Clicked AddButton");
       
       // ... rest of test
   }
   ```

### Optimizing Pipeline Performance

1. **Use Caching**:
   - Cache NuGet packages
   - Cache build outputs (if applicable)

2. **Parallelize Tests** (if feasible):
   ```yaml
   strategy:
     parallel: 2
   ```

3. **Skip Unnecessary Steps**:
   - Only restore dependencies if `packages.lock.json` changed

### Monitoring Pipeline Health

1. **Set Up Notifications**:
   - Azure DevOps ? Project Settings ? Notifications
   - Configure email alerts for pipeline failures

2. **Track Test Trends**:
   - Go to Pipelines ? Analytics
   - View test pass rate over time
   - Identify flaky tests

3. **Review Pipeline History**:
   - Check for patterns in failures
   - Identify frequently failing tests

---

## Summary

### What This Pipeline Does
1. ? Automatically builds the StudentDataViewer WPF application
2. ? Installs WinAppDriver on a fresh Windows VM
3. ? Runs all automated UI tests from StudentDataViewerTest
4. ? Publishes test results with detailed analytics
5. ? Uploads artifacts for debugging failed tests
6. ? Provides fast feedback on code changes

### Key Benefits
- **Automated Testing**: Every push triggers tests automatically
- **Early Bug Detection**: Catch UI regressions before merging
- **Consistent Environment**: Tests run on a clean Windows VM every time
- **Detailed Reporting**: View test results, trends, and failure diagnostics
- **Easy Debugging**: Download artifacts (screenshots, logs) for failed tests

### Next Steps
1. ? Review the pipeline configuration in `azure-pipelines.yml`
2. ? Test locally using `run-ui-tests.ps1`
3. ? Push code to `wpf_training_TestCaseAutomation` branch
4. ? Monitor pipeline execution in Azure DevOps
5. ? Address any failures using the troubleshooting guide
6. ? Set up notifications for pipeline status

---

## Additional Resources

- **Generic Pipeline Guide**: See `docs/AZURE_DEVOPS_PIPELINE_SETUP.md` for general concepts
- **WinAppDriver Setup**: See `WINAPPDRIVER_SETUP_SUMMARY.md` for local setup
- **Test Script**: See `run-ui-tests.ps1` for local test execution
- **Test Files**: See `tests/unit/StudentDataViewerTest/*.cs` for test implementations

---

## Support

If you encounter issues not covered in this guide:

1. **Check Azure DevOps Logs**:
   - View detailed step-by-step output in the pipeline run

2. **Review Test Results**:
   - Check the Tests tab for specific failure messages

3. **Download Artifacts**:
   - Get screenshots and logs from failed runs

4. **Test Locally**:
   - Run `run-ui-tests.ps1` to reproduce issues

5. **Consult Documentation**:
   - [Azure Pipelines Docs](https://docs.microsoft.com/azure/devops/pipelines/)
   - [WinAppDriver GitHub](https://github.com/microsoft/WinAppDriver)

---

**Last Updated**: 2025
**Pipeline Version**: 1.0
**WinAppDriver Version**: 1.2.1
**Target Framework**: .NET 8, .NET Framework 4.8
