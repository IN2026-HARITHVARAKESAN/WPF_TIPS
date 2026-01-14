namespace TableDataVisualizer
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    public partial class VisualizerForm : Form
    {
        public VisualizerForm()
        {
            InitializeComponent();
            this.Text = "Table Data Visualizer";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            this.Controls.Add(dataGridView);
        }

        public void DisplayData(DataTable dataTable)
        {
            if (Controls.Count > 0 && Controls[0] is DataGridView grid)
            {
                grid.DataSource = dataTable;
            }
        }

        public void DisplayData<T>(IEnumerable<T> collection)
        {
            if (Controls.Count > 0 && Controls[0] is DataGridView grid)
            {
                var dataTable = ConvertToDataTable(collection);
                grid.DataSource = dataTable;
            }
        }

        private DataTable ConvertToDataTable<T>(IEnumerable<T> collection)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in collection)
            {
                var row = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}