using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;

namespace StandaloneVisualizer
{
    public partial class VisualizerWindow : Window
    {
        private DataTable? _currentDataTable;

        public VisualizerWindow()
        {
            InitializeComponent();
            StatusText.Text = "Initializing...";
        }

        /// <summary>
        /// Display data from any IEnumerable collection
        /// </summary>
        public void DisplayData(IEnumerable data, string title = "Data Visualizer", string subtitle = "")
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = title;
                TitleTextBlock.Text = title;
            }

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                SubtitleTextBlock.Text = subtitle;
            }

            try
            {
                _currentDataTable = ConvertToDataTable(data);
                DataGrid.ItemsSource = _currentDataTable.DefaultView;
                UpdateStatus(_currentDataTable.Rows.Count, _currentDataTable.Columns.Count);
                StatusText.Text = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to display data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Display strongly-typed collection
        /// </summary>
        public void DisplayData<T>(IEnumerable<T> data, string title = "Data Visualizer", string subtitle = "")
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = title;
                TitleTextBlock.Text = title;
            }

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                SubtitleTextBlock.Text = subtitle;
            }

            try
            {
                _currentDataTable = ConvertToDataTable(data);
                DataGrid.ItemsSource = _currentDataTable.DefaultView;
                UpdateStatus(_currentDataTable.Rows.Count, _currentDataTable.Columns.Count);
                StatusText.Text = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to display data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Display DataTable directly
        /// </summary>
        public void DisplayData(DataTable dataTable, string title = "Data Visualizer", string subtitle = "")
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = title;
                TitleTextBlock.Text = title;
            }

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                SubtitleTextBlock.Text = subtitle;
            }

            try
            {
                _currentDataTable = dataTable;
                DataGrid.ItemsSource = dataTable.DefaultView;
                UpdateStatus(dataTable.Rows.Count, dataTable.Columns.Count);
                StatusText.Text = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to display data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatus(int rowCount, int columnCount)
        {
            RowCountText.Text = rowCount.ToString();
            ColumnCountText.Text = columnCount.ToString();
        }

        private DataTable ConvertToDataTable(IEnumerable collection)
        {
            var dataTable = new DataTable();

            if (collection == null)
            {
                StatusText.Text = "No data provided";
                return dataTable;
            }

            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                StatusText.Text = "Empty collection";
                return dataTable;
            }

            var firstItem = enumerator.Current;
            if (firstItem == null)
            {
                StatusText.Text = "Collection contains null items";
                return dataTable;
            }

            var properties = firstItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            foreach (var prop in properties)
            {
                var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, columnType);
            }

            AddRowToDataTable(dataTable, firstItem, properties);

            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                    AddRowToDataTable(dataTable, enumerator.Current, properties);
            }

            return dataTable;
        }

        private DataTable ConvertToDataTable<T>(IEnumerable<T> collection)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToArray();

            foreach (var prop in properties)
            {
                var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, columnType);
            }

            foreach (var item in collection)
            {
                if (item != null)
                    AddRowToDataTable(dataTable, item, properties);
            }

            return dataTable;
        }

        private void AddRowToDataTable(DataTable dataTable, object item, PropertyInfo[] properties)
        {
            var row = dataTable.NewRow();
            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(item);
                    row[prop.Name] = value ?? DBNull.Value;
                }
                catch
                {
                    row[prop.Name] = DBNull.Value;
                }
            }
            dataTable.Rows.Add(row);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDataTable == null || _currentDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportToCsv(_currentDataTable, saveFileDialog.FileName);
                    StatusText.Text = "Data exported successfully";
                    MessageBox.Show($"Data exported to:\n{saveFileDialog.FileName}", "Export Successful", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    StatusText.Text = "Export failed";
                    MessageBox.Show($"Failed to export data:\n{ex.Message}", "Export Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportToCsv(DataTable dataTable, string filePath)
        {
            var csv = new StringBuilder();

            var columnNames = dataTable.Columns.Cast<DataColumn>()
                .Select(column => EscapeCsvValue(column.ColumnName));
            csv.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field => 
                    EscapeCsvValue(field?.ToString() ?? string.Empty));
                csv.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }

    /// <summary>
    /// Static helper class to easily show the visualizer from any project
    /// </summary>
    public static class DataVisualizer
    {
        public static void Show(IEnumerable data, string title = "Data Visualizer", string subtitle = "")
        {
            var window = new VisualizerWindow();
            window.DisplayData(data, title, subtitle);
            window.ShowDialog();
        }

        public static void Show<T>(IEnumerable<T> data, string title = "Data Visualizer", string subtitle = "")
        {
            var window = new VisualizerWindow();
            window.DisplayData(data, title, subtitle);
            window.ShowDialog();
        }

        public static void Show(DataTable dataTable, string title = "Data Visualizer", string subtitle = "")
        {
            var window = new VisualizerWindow();
            window.DisplayData(dataTable, title, subtitle);
            window.ShowDialog();
        }
    }
}
