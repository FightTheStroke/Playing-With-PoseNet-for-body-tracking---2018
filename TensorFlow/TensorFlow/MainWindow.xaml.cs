using ExampleCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TensorFlow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoadModel(object sender, RoutedEventArgs e)
        {
            PoseNet posenet = new PoseNet();

            using (var graph = new TFGraph())
            {
                graph.Import(File.ReadAllBytes("Resources\\frozen_model.bytes"));
                var session = new TFSession(graph);
                string input = "photo.jpg";
                var tensor = ImageUtil.CreateTensorFromImageFile(input);
                var runner = session.GetRunner();

                runner.AddInput(graph["image"][0], tensor);
                runner.Fetch(
             graph["heatmap"][0],
             graph["offset_2"][0],
             graph["displacement_fwd_2"][0],
             graph["displacement_bwd_2"][0]
         );
                var result = runner.Run();
                var heatmap = (float[,,,])result[0].GetValue(jagged: false);
                var offsets = (float[,,,])result[1].GetValue(jagged: false);
                var displacementsFwd = (float[,,,])result[2].GetValue(jagged: false);
                var displacementsBwd = (float[,,,])result[3].GetValue(jagged: false);

                var poses = posenet.DecodeMultiplePoses(
           heatmap, offsets,
           displacementsFwd,
           displacementsBwd,
           outputStride: 16, maxPoseDetections: 15,
           scoreThreshold: 0.001f, nmsRadius: 20);
            }
        }
    }
}
