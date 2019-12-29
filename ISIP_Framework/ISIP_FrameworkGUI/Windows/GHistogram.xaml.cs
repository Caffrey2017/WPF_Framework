using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using ISIP_FrameworkHelpers;
using Emgu.CV;
using Emgu.CV.Structure;
using ISIP_Algorithms.Tools;

namespace ISIP_FrameworkGUI.Windows
{
    /// <summary>
    /// Interaction logic for Histogram.xaml
    /// </summary>
    public partial class GHistogram : Window
    {
        Image<Gray, byte> InputImage;
        Image<Gray, byte> OutputImage;
        public GHistogram(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            InitializeComponent();
            InputImage = image1;
            OutputImage = image2;
        }
        
        public void Redraw()
        {
            List<Point> gLevelsOriginal = new List<Point>();
            List<Point> gLevelsProcessed = new List<Point>();
            Histogram.Children.Clear();

            int[] grayLevelsOriginal = new int[256];
            grayLevelsOriginal = Tools.GenerateHistogram(InputImage);
            
            for (int i = 0; i < grayLevelsOriginal.Length; i++)
            {
                int y = (int)(grayLevelsOriginal[i] / Histogram.ActualHeight) + 30;
                gLevelsOriginal.Add(new Point(i + 30, Histogram.ActualHeight - 10 - y));
            }
            DrawHelper.DrawAndGetPolyline(Histogram, gLevelsOriginal, Brushes.Blue, 1);


            int[] grayLevelsProcessed = new int[256];
            grayLevelsProcessed = Tools.GenerateHistogram(OutputImage);
            for (int i = 0; i < grayLevelsProcessed.Length; i++)
            {
                int y = (int)(grayLevelsProcessed[i] / Histogram.ActualHeight + 30);
                gLevelsProcessed.Add(new Point(i + 30, Histogram.ActualHeight - 10 - y));
            }
            DrawHelper.DrawAndGetPolyline(Histogram, gLevelsProcessed, Brushes.Red, 1);
        }
    }
}
