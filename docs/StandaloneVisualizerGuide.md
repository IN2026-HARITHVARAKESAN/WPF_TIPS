# Standalone Visualizer - Usage Guide

## Overview
The Standalone Visualizer is a separate WPF project that provides a clean, professional data grid viewer for displaying any collection of data in a table format.

## Features
- ? Display any `IEnumerable` collection
- ? Automatic column generation from object properties
- ? Professional UI with styled headers and alternating rows
- ? Export to CSV functionality
- ? Sort columns by clicking headers
- ? Resize and reorder columns
- ? Status bar showing row and column counts
- ? Clean, modern design with color-coded elements

## Project Structure
```
src/StandaloneVisualizer/
??? StandaloneVisualizer.csproj    # Project file
??? VisualizerWindow.xaml          # UI definition
??? VisualizerWindow.xaml.cs       # Code-behind with logic
```

## Integration with StudentDataViewer

### 1. Project Reference
The StudentDataViewer project references StandaloneVisualizer:

```xml
<ItemGroup>
  <ProjectReference Include="..\StandaloneVisualizer\StandaloneVisualizer.csproj" />
</ItemGroup>
```

### 2. Command Binding
In `MainViewModel.cs`:
```csharp
using StandaloneVisualizer;

public ICommand ShowVisualizerCommand { get; }

// In constructor
ShowVisualizerCommand = new RelayCommand(ShowVisualizer, CanShowVisualizer);

// Methods
private void ShowVisualizer(object? parameter)
{
    var dataToShow = StudentsToDisplay?.Count > 0 ? StudentsToDisplay : Students;
    var subtitle = SelectedSection != null && SelectedSection != "ALL" 
        ? $"Section: {SelectedSection} | Total Records: {dataToShow.Count}"
        : $"All Sections | Total Records: {dataToShow.Count}";
    
    DataVisualizer.Show(dataToShow, "Student Data Visualizer", subtitle);
}

private bool CanShowVisualizer(object? parameter)
{
    return Students?.Count > 0;
}
```

### 3. UI Button
In `LeftPanelUserControl.xaml`:
```xaml
<Button x:Name="ShowVisualizerButton" 
        AutomationProperties.AutomationId="ShowVisualizerButton" 
        Width="145" Height="30" 
        Command="{Binding DataContext.ShowVisualizerCommand, ElementName=RootControl}">
    <Border Background="#9B59B6" CornerRadius="10" BorderThickness="2" Padding="10,5">
        <TextBlock Text="?? View Data" Foreground="{StaticResource StandardWhite}"/>
    </Border>
</Button>
```

## Usage Examples

### Example 1: Simple Usage
```csharp
using StandaloneVisualizer;

// Show any collection
DataVisualizer.Show(myStudentList, "Student List");
```

### Example 2: With Custom Title and Subtitle
```csharp
var students = GetStudents();
DataVisualizer.Show(
    students, 
    "Student Database", 
    $"Showing {students.Count} records"
);
```

### Example 3: From Button Click
```csharp
private void ViewDataButton_Click(object sender, RoutedEventArgs e)
{
    DataVisualizer.Show(
        StudentsToDisplay, 
        "Filtered Students", 
        $"Section: {SelectedSection}"
    );
}
```

### Example 4: Using Window Instance
```csharp
var window = new VisualizerWindow();
window.DisplayData(myCollection, "My Data", "Custom subtitle");
window.ShowDialog(); // Modal
// or
window.Show(); // Non-modal
```

## How to Use in Your Project

### Step 1: Add Project to Solution
```bash
dotnet sln add src/StandaloneVisualizer/StandaloneVisualizer.csproj
```

### Step 2: Add Project Reference
```bash
dotnet add YourProject.csproj reference ../StandaloneVisualizer/StandaloneVisualizer.csproj
```

Or manually edit your `.csproj`:
```xml
<ItemGroup>
  <ProjectReference Include="..\StandaloneVisualizer\StandaloneVisualizer.csproj" />
</ItemGroup>
```

### Step 3: Use in Code
```csharp
using StandaloneVisualizer;

// One line to show any data!
DataVisualizer.Show(yourCollection, "Your Title");
```

## API Reference

### Static Helper Class: `DataVisualizer`

#### Method 1: Show Generic Collection
```csharp
public static void Show<T>(
    IEnumerable<T> data, 
    string title = "Data Visualizer", 
    string subtitle = ""
)
```

#### Method 2: Show Non-Generic Collection
```csharp
public static void Show(
    IEnumerable data, 
    string title = "Data Visualizer", 
    string subtitle = ""
)
```

#### Method 3: Show DataTable
```csharp
public static void Show(
    DataTable dataTable, 
    string title = "Data Visualizer", 
    string subtitle = ""
)
```

### Instance Class: `VisualizerWindow`

#### Constructor
```csharp
public VisualizerWindow()
```

#### Display Methods
```csharp
public void DisplayData<T>(IEnumerable<T> data, string title, string subtitle)
public void DisplayData(IEnumerable data, string title, string subtitle)
public void DisplayData(DataTable dataTable, string title, string subtitle)
```

## Features in Detail

### 1. Auto-Column Generation
The visualizer automatically detects all public properties of your objects and creates columns.

### 2. Export to CSV
Click the "Export to CSV" button in the status bar to save the current view to a CSV file.

### 3. Sorting
Click any column header to sort by that column.

### 4. Column Operations
- **Resize**: Drag column borders
- **Reorder**: Drag column headers
- **Sort**: Click headers

### 5. Selection
- Select single rows
- Multi-select with Ctrl/Shift
- Select all rows visible

## Styling

### Color Scheme
- **Header Background**: `#2C3E50` (Dark blue-gray)
- **Column Headers**: `#34495E` (Medium blue-gray)
- **Selected Rows**: `#3498DB` (Blue)
- **Export Button**: `#27AE60` (Green)
- **Alternating Rows**: `#F5F5F5` (Light gray)

### Customization
To customize colors, edit `VisualizerWindow.xaml`:
```xaml
<Border Background="#YourColor">
```

## Troubleshooting

### Issue: "Type or namespace 'StandaloneVisualizer' could not be found"
**Solution**: Restore and rebuild the solution:
```bash
dotnet restore
dotnet build
```

### Issue: Export button not working
**Solution**: Ensure the window has data loaded before exporting.

### Issue: Empty window appears
**Solution**: Check that you're passing a non-null, non-empty collection.

## Benefits

### For StudentDataViewer Project
- ? Clean separation of concerns
- ? Reusable across multiple views
- ? Professional data presentation
- ? Export functionality out-of-the-box
- ? No need to create custom data grids

### For Other Projects
- ? Drop-in solution for data viewing
- ? Works with any object type
- ? No configuration needed
- ? Modern, professional UI

## Example: Full Integration

```csharp
// In ViewModel
using System.Collections.ObjectModel;
using System.Windows.Input;
using StandaloneVisualizer;

public class MyViewModel
{
    public ObservableCollection<Student> Students { get; set; }
    public ICommand ShowDataCommand { get; }
    
    public MyViewModel()
    {
        Students = new ObservableCollection<Student>();
        ShowDataCommand = new RelayCommand(ShowData);
    }
    
    private void ShowData(object? parameter)
    {
        DataVisualizer.Show(
            Students, 
            "Student Records", 
            $"Total: {Students.Count} students"
        );
    }
}
```

```xaml
<!-- In XAML -->
<Button Content="?? View Data" 
        Command="{Binding ShowDataCommand}" />
```

## Summary
The Standalone Visualizer provides a professional, ready-to-use solution for displaying tabular data in WPF applications. It's completely separate from your main project, making it easy to reuse and maintain.

**One line of code to visualize any data:**
```csharp
DataVisualizer.Show(yourData, "Your Title");
```
