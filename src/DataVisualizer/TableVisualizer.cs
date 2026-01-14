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

            var data = objectProvider.GetObject();

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

        private DataTable ConvertToDataTable(SerializableCollection collection)
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

                    collection.Items.Add(dict);
                }
            }

            JsonSerializer.Serialize(outgoingData, collection);
        }
    }
}