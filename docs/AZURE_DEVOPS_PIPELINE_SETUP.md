# Azure DevOps Pipeline Setup for WinAppDriver Tests

This guide explains how to set up Azure DevOps pipelines for automated WinAppDriver UI testing.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Pipeline Configuration Overview](#pipeline-configuration-overview)
- [Step-by-Step Setup](#step-by-step-setup)
- [Pipeline Components Explained](#pipeline-components-explained)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before setting up the pipeline, ensure you have:
- An Azure DevOps account and project
- A Git repository with your WPF application
- WinAppDriver-compatible UI tests (e.g., using Appium.WebDriver)
- A Windows-based application (WinAppDriver only supports Windows)

---

## Pipeline Configuration Overview

The Azure DevOps pipeline automates the following workflow:
1. **Checkout**: Clone the repository code
2. **Setup**: Install .NET SDK and dependencies
3. **Build**: Compile the WPF application
4. **Install WinAppDriver**: Download and install the UI automation driver
5. **Run Tests**: Execute automated UI tests
6. **Publish Results**: Upload test results and artifacts

---

## Step-by-Step Setup

### Step 1: Clone Your Repository Locally
```bash
git clone <your-repository-url>
cd <your-repository-folder>
```

### Step 2: Create the YAML Pipeline File

Create a file named `azure-pipelines.yml` in the root folder of your repository.

### Step 3: Configure the Pipeline File

Copy the template below and customize it for your project:

```yaml
# ============================================
# TRIGGER CONFIGURATION
# ============================================
# Defines when the pipeline should run automatically
trigger:
  branches:
    include:
      - main                    # Replace with your branch names
      - develop
  paths:
    include:
      - src/**                  # Triggers when source code changes
      - tests/unit/**           # Triggers when tests change

# ============================================
# PULL REQUEST CONFIGURATION (Optional)
# ============================================
# Use 'pr' instead of 'trigger' if you want the pipeline 
# to run ONLY when a Pull Request is created
# pr:
#   branches:
#     include:
#       - main
#   paths:
#     include:
#       - src/**
#       - tests/unit/**

# ============================================
# AGENT POOL CONFIGURATION
# ============================================
# WinAppDriver requires Windows, so use a Windows VM
pool:
  vmImage: 'windows-latest'

# ============================================
# VARIABLES
# ============================================
variables:
  buildConfiguration: 'Release'
  solution: '**/*.sln'
  testProject: 'tests/unit/YourTestProject/YourTestProject.csproj'
  appProject: 'src/YourApp/YourApp.csproj'
  appOutputPath: '$(Build.SourcesDirectory)/build/YourApp'

# ============================================
# PIPELINE STAGES
# ============================================
stages:
  - stage: Build
    displayName: 'Build and Test'
    jobs:
      - job: WinAppDriverTests
        displayName: 'Run WinAppDriver UI Tests'
        steps:

          # ========================================
          # STEP 1: Checkout Code
          # ========================================
          - checkout: self
            displayName: 'Checkout code'

          # ========================================
          # STEP 2: Setup .NET SDK
          # ========================================
          - task: UseDotNet@2
            displayName: 'Setup .NET 8'
            inputs:
              packageType: 'sdk'
              version: '8.0.x'
              installationPath: $(Agent.ToolsDirectory)/dotnet

          # ========================================
          # STEP 3: Restore Dependencies
          # ========================================
          - task: DotNetCoreCLI@2
            displayName: 'Restore dependencies'
            inputs:
              command: 'restore'
              projects: '$(solution)'

          # ========================================
          # STEP 4: Build WPF Application
          # ========================================
          - task: PowerShell@2
            displayName: 'Build WPF Application'
            inputs:
              targetType: 'inline'
              script: |
                dotnet build $(appProject) --configuration $(buildConfiguration) --output $(appOutputPath)
                Write-Host "Build completed"

          # ========================================
          # STEP 5: Verify EXE File Exists
          # ========================================
          - task: PowerShell@2
            displayName: 'Verify EXE file exists'
            inputs:
              targetType: 'inline'
              script: |
                $exePath = "$(appOutputPath)/YourApp.exe"
                if (Test-Path $exePath) {
                  Write-Host "##[section]? EXE file found at: $exePath"
                  Get-Item $exePath | Format-List
                } else {
                  Write-Host "##[error]? EXE file not found!"
                  Write-Host "Directory contents:"
                  Get-ChildItem "$(appOutputPath)" -Recurse
                  exit 1
                }

          # ========================================
          # STEP 6: Download and Install WinAppDriver
          # ========================================
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

          # ========================================
          # STEP 7: Start WinAppDriver
          # ========================================
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

          # ========================================
          # STEP 8: Update Test Configuration
          # ========================================
          - task: PowerShell@2
            displayName: 'Update Test Configuration'
            inputs:
              targetType: 'inline'
              script: |
                $appPath = "$(appOutputPath)/YourApp.exe"
                
                # Create test settings
                $testSettings = @{
                  AppPath = $appPath
                } | ConvertTo-Json
                
                $testSettings | Out-File -FilePath "tests/unit/YourTestProject/testsettings.json" -Encoding UTF8
                
                Write-Host "Test settings created with AppPath: $appPath"

          # ========================================
          # STEP 9: Run WinAppDriver Tests
          # ========================================
          - task: PowerShell@2
            displayName: 'Run WinAppDriver Tests'
            inputs:
              targetType: 'inline'
              script: |
                $env:TEST_APP_PATH = "$(appOutputPath)/YourApp.exe"
                Write-Host "Running UI tests with app path: $env:TEST_APP_PATH"
                
                dotnet test $(testProject) `
                  --configuration $(buildConfiguration) `
                  --no-build `
                  --logger "trx;LogFileName=test-results.trx" `
                  --logger "console;verbosity=detailed"
            continueOnError: true

          # ========================================
          # STEP 10: Stop WinAppDriver
          # ========================================
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

          # ========================================
          # STEP 11: Publish Test Results
          # ========================================
          - task: PublishTestResults@2
            displayName: 'Publish Test Results'
            condition: always()
            inputs:
              testResultsFormat: 'VSTest'
              testResultsFiles: '**/test-results.trx'
              mergeTestResults: true
              failTaskOnFailedTests: true
              testRunTitle: 'WinAppDriver UI Tests'

          # ========================================
          # STEP 12: Upload Test Artifacts (on failure)
          # ========================================
          - task: PublishPipelineArtifact@1
            displayName: 'Upload Test Artifacts'
            condition: failed()
            inputs:
              targetPath: '$(Build.SourcesDirectory)/tests/unit/YourTestProject/TestResults'
              artifact: 'test-results'
              publishLocation: 'pipeline'
```

### Step 4: Customize the Pipeline

Replace the following placeholders with your project-specific values:

| Placeholder | Description | Example |
|-------------|-------------|---------|
| `main`, `develop` | Your branch names | `main`, `feature-branch` |
| `YourTestProject` | Your test project name | `MyApp.Tests` |
| `YourApp` | Your application project name | `MyWpfApp` |
| `YourApp.exe` | Your executable name | `MyWpfApp.exe` |
| `src/**` | Path to source code | `src/MyApp/**` |
| `tests/unit/**` | Path to tests | `tests/integration/**` |

### Step 5: Commit and Push

```bash
git add azure-pipelines.yml
git commit -m "Add Azure DevOps pipeline for WinAppDriver tests"
git push origin main
```

### Step 6: Create Pipeline in Azure DevOps

1. Navigate to your Azure DevOps project
2. Go to **Pipelines** ? **New Pipeline**
3. Select **Azure Repos Git** (or your repository location)
4. Choose **Existing Azure Pipelines YAML file**
5. Select `azure-pipelines.yml` from the branch dropdown
6. Click **Run**

---

## Pipeline Components Explained

### 1. Trigger Configuration

**Purpose**: Defines when the pipeline runs automatically.

```yaml
trigger:
  branches:
    include:
      - main
      - develop
  paths:
    include:
      - src/**
      - tests/unit/**
```

- **branches.include**: Pipeline runs when changes are pushed to these branches
- **paths.include**: Pipeline runs only if changes are detected in these paths
- If no changes match the paths, the pipeline is skipped

### 2. Pull Request Trigger (Alternative)

**Purpose**: Run pipeline only when a PR is created.

```yaml
pr:
  branches:
    include:
      - main
  paths:
    include:
      - src/**
      - tests/unit/**
```

Use `pr` instead of `trigger` to run the pipeline exclusively for pull requests targeting the specified branches.

### 3. Agent Pool

**Purpose**: Specifies the VM environment for running the pipeline.

```yaml
pool:
  vmImage: 'windows-latest'
```

- **Why Windows?**: WinAppDriver is Windows-only
- Azure DevOps provisions a Windows VM, runs the pipeline, and destroys the VM afterward

### 4. Variables

**Purpose**: Define reusable values throughout the pipeline.

```yaml
variables:
  buildConfiguration: 'Release'
  solution: '**/*.sln'
  testProject: 'tests/unit/YourTestProject/YourTestProject.csproj'
```

- Makes the pipeline easier to maintain
- Change values in one place instead of multiple steps

### 5. Build Stage

**Purpose**: Orchestrates the build and test process.

```yaml
stages:
  - stage: Build
    displayName: 'Build and Test'
    jobs:
      - job: WinAppDriverTests
        displayName: 'Run WinAppDriver UI Tests'
        steps:
          - checkout: self
            displayName: 'Checkout code'
```

- **checkout: self**: Copies repository code to the agent VM

### 6. Setup .NET SDK

**Purpose**: Installs the required .NET SDK version.

```yaml
- task: UseDotNet@2
  displayName: 'Setup .NET 8'
  inputs:
    packageType: 'sdk'
    version: '8.0.x'
```

### 7. Build Application

**Purpose**: Compiles the WPF application to generate the executable.

```yaml
- task: PowerShell@2
  displayName: 'Build WPF Application'
  inputs:
    targetType: 'inline'
    script: |
      dotnet build $(appProject) --configuration $(buildConfiguration) --output $(appOutputPath)
```

### 8. Install WinAppDriver

**Purpose**: Downloads and installs WinAppDriver on the agent VM.

```yaml
- task: PowerShell@2
  displayName: 'Download and Install WinAppDriver'
  inputs:
    targetType: 'inline'
    script: |
      $winAppDriverUrl = "https://github.com/microsoft/WinAppDriver/releases/download/v1.2.1/WindowsApplicationDriver.msi"
      $installerPath = "$env:TEMP\WinAppDriver.msi"
      Invoke-WebRequest -Uri $winAppDriverUrl -OutFile $installerPath
      Start-Process msiexec.exe -ArgumentList "/i", $installerPath, "/quiet", "/norestart" -Wait
```

### 9. Run Tests

**Purpose**: Executes the WinAppDriver UI tests.

```yaml
- task: PowerShell@2
  displayName: 'Run WinAppDriver Tests'
  inputs:
    targetType: 'inline'
    script: |
      dotnet test $(testProject) --configuration $(buildConfiguration) --logger "trx;LogFileName=test-results.trx"
  continueOnError: true
```

- **continueOnError: true**: Pipeline continues even if tests fail (to publish results)

### 10. Publish Test Results

**Purpose**: Uploads test results to Azure DevOps for visualization.

```yaml
- task: PublishTestResults@2
  displayName: 'Publish Test Results'
  condition: always()
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/test-results.trx'
    failTaskOnFailedTests: true
```

- **condition: always()**: Runs even if previous steps fail
- **failTaskOnFailedTests: true**: Pipeline fails if any tests fail

---

## Troubleshooting

### Common Issues

#### 1. Pipeline Not Triggering
- **Problem**: Pipeline doesn't run after pushing code
- **Solution**: Check that your branch and paths match the trigger configuration

#### 2. WinAppDriver Not Starting
- **Problem**: Tests fail because WinAppDriver isn't running
- **Solution**: Add a longer sleep duration after starting WinAppDriver:
  ```yaml
  Start-Sleep -Seconds 10
  ```

#### 3. EXE Not Found
- **Problem**: Build succeeds but executable is not found
- **Solution**: Verify the output path matches your project structure:
  ```yaml
  dotnet build --output $(Build.SourcesDirectory)/build/YourApp
  ```

#### 4. Tests Fail with "Element Not Found"
- **Problem**: UI elements are not ready when tests run
- **Solution**: Add implicit waits in your test code or increase timeouts

#### 5. Permission Issues
- **Problem**: WinAppDriver cannot start the application
- **Solution**: Ensure the EXE has execute permissions and the path is correct

### Debugging Tips

1. **Enable Verbose Logging**:
   ```yaml
   --logger "console;verbosity=detailed"
   ```

2. **Capture Screenshots on Failure**:
   Add code in your tests to save screenshots to `TestResults` folder

3. **Review Build Artifacts**:
   Check the `test-results` artifact in Azure DevOps for detailed logs

4. **Test Locally First**:
   Run the pipeline steps manually on your local machine to verify they work

---

## Additional Resources

- [Azure Pipelines Documentation](https://docs.microsoft.com/azure/devops/pipelines/)
- [WinAppDriver GitHub](https://github.com/microsoft/WinAppDriver)
- [YAML Schema Reference](https://docs.microsoft.com/azure/devops/pipelines/yaml-schema)

---

## Summary

This pipeline automates the entire WinAppDriver testing workflow:
1. ? Checks out code from your repository
2. ? Installs .NET SDK and restores dependencies
3. ? Builds your WPF application
4. ? Downloads and installs WinAppDriver
5. ? Runs automated UI tests
6. ? Publishes test results for review
7. ? Uploads artifacts on failure for debugging

By following this guide, you can set up continuous integration for your WPF application with automated UI testing.
