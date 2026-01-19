using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(TableDataVisualizer.TableVisualizer),
    typeof(VisualizerObjectSource),
    Target = typeof(DataTable),
    Description = "Table Data Visualizer")]

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(TableDataVisualizer.TableVisualizer),
    typeof(TableDataVisualizer.EnumerableObjectSource),
    Target = typeof(IEnumerable<>),
    Description = "Collection Table Visualizer")]

namespace TableDataVisualizer
{
    [Obsolete]
    public class TableVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService));
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            // Deserialize using JSON (modern approach)
            object data;
            using (var stream = objectProvider.GetData())
            {
                data = JsonSerializer.Deserialize<SerializableCollection>(stream);
            }

            using (var form = new VisualizerForm())
            {
                if (data is DataTable dt)
                {
                    form.DisplayData(dt);
                }
                else if (data is SerializableCollection serializableCollection)
                {
                    var collectionDataTable = ConvertToDataTable(serializableCollection);
                    form.DisplayData(collectionDataTable);
                }

                windowService.ShowDialog(form);
            }
        }

        /// <summary>
        /// Public static method to show visualizer directly from code
        /// This bypasses Visual Studio debugger integration
        /// </summary>
        public static void ShowVisualizer<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var serializableCollection = new SerializableCollection();

            foreach (var item in collection)
            {
                if (item == null) continue;

                var dict = new Dictionary<string, object>();
                
                // Handle primitive types (string, int, etc.)
                if (item.GetType().IsPrimitive || item is string || item is decimal || item is DateTime)
                {
                    dict["Value"] = item;
                }
                else
                {
                    // Handle complex types with properties
                    var properties = item.GetType().GetProperties();

                    foreach (var prop in properties)
                    {
                        try
                        {
                            var value = prop.GetValue(item);
                            dict[prop.Name] = value;
                        }
                        catch
                        {
                            dict[prop.Name] = "[Error reading property]";
                        }
                    }
                }

                serializableCollection.Items.Add(dict);
            }

            var visualizer = new TableVisualizer();
            var dataTable = visualizer.ConvertToDataTable(serializableCollection);

            using (var form = new VisualizerForm())
            {
                form.DisplayData(dataTable);
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Public static method to show DataTable directly
        /// </summary>
        public static void ShowVisualizer(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            using (var form = new VisualizerForm())
            {
                form.DisplayData(dataTable);
                form.ShowDialog();
            }
        }

        // Make this method public so it can be called from static methods
        public DataTable ConvertToDataTable(SerializableCollection collection)
        {
            var dataTable = new DataTable();

            if (collection.Items.Count == 0)
                return dataTable;

            // Add columns based on first item's properties
            foreach (var kvp in collection.Items[0])
            {
                dataTable.Columns.Add(kvp.Key);
            }

            // Add rows
            foreach (var item in collection.Items)
            {
                var row = dataTable.NewRow();
                foreach (var kvp in item)
                {
                    row[kvp.Key] = kvp.Value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }

    [Serializable]
    public class SerializableCollection
    {
        public List<Dictionary<string, object>> Items { get; set; } = new List<Dictionary<string, object>>();
    }

    public class EnumerableObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var collection = new SerializableCollection();

            if (target is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    var dict = new Dictionary<string, object>();
                    
                    // Handle primitive types (string, int, etc.)
                    if (item.GetType().IsPrimitive || item is string || item is decimal || item is DateTime)
                    {
                        dict["Value"] = item;
                    }
                    else
                    {
                        // Handle complex types with properties
                        var properties = item.GetType().GetProperties();

                        foreach (var prop in properties)
                        {
                            try
                            {
                                var value = prop.GetValue(item);
                                dict[prop.Name] = value;
                            }
                            catch
                            {
                                dict[prop.Name] = "[Error reading property]";
                            }
                        }
                    }

                    collection.Items.Add(dict);
                }
            }

            JsonSerializer.Serialize(outgoingData, collection);
        }
    }
}