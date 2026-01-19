using System.Collections.Generic;
using System.Windows;
using TableDataVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            List<string> abc = new List<string>() { "abc", "agb" };

            // Use VisualizerDevelopmentHost with all 3 required parameters
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(
                abc,                              // Object to visualize
                typeof(TableVisualizer),          // Visualizer type
                typeof(EnumerableObjectSource)    // Object source for serialization
            );

            visualizerHost.ShowVisualizer();
            //TableVisualizer.ShowVisualizer(abc);

        }
    }
}