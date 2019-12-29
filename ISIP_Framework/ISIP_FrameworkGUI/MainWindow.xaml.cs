using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ISIP_UserControlLibrary;

using ISIP_Algorithms.Tools;
using ISIP_FrameworkHelpers;
using ISIP_Algorithms.Help;
using ISIP_FrameworkGUI.Windows;

namespace ISIP_FrameworkGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        //private Windows.Grafica dialog;
        Windows.Magnifyer MagnifWindow;
        Windows.GLine RowDisplay;
        bool Magif_SHOW = false;
        bool GL_ROW_SHOW = false;
        bool Histogram_SHOW = false;
        System.Windows.Point lastClick = new System.Windows.Point(0, 0);
        System.Windows.Point upClick = new System.Windows.Point(0, 0);

        // my properties
        Windows.GHistogram HistogramDisplay;

        public MainWindow()
        {
            InitializeComponent();
            mainControl.OriginalImageCanvas.MouseDown += new MouseButtonEventHandler(OriginalImageCanvas_MouseDown);
        }

        void OriginalImageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastClick = Mouse.GetPosition(mainControl.OriginalImageCanvas);
            DrawHelper.RemoveAllLines(mainControl.OriginalImageCanvas);
            DrawHelper.RemoveAllRectangles(mainControl.OriginalImageCanvas);
            DrawHelper.RemoveAllLines(mainControl.ProcessedImageCanvas);
            DrawHelper.RemoveAllRectangles(mainControl.ProcessedImageCanvas);
            if (GL_ROW_ON.IsChecked)
            {
                DrawHelper.DrawAndGetLine(mainControl.OriginalImageCanvas, new System.Windows.Point(0, lastClick.Y),
                     new System.Windows.Point(mainControl.OriginalImageCanvas.Width - 1, lastClick.Y), System.Windows.Media.Brushes.Red, 1);
                if (mainControl.ProcessedGrayscaleImage != null)
                {
                    DrawHelper.DrawAndGetLine(mainControl.ProcessedImageCanvas, new System.Windows.Point(0, lastClick.Y),
                     new System.Windows.Point(mainControl.ProcessedImageCanvas.Width - 1, lastClick.Y), System.Windows.Media.Brushes.Red, 1);
                }
                if (mainControl.OriginalGrayscaleImage != null) RowDisplay.Redraw((int)lastClick.Y);

            }
            if (Magnifyer_ON.IsChecked)
            {
                DrawHelper.DrawAndGetLine(mainControl.OriginalImageCanvas, new System.Windows.Point(0, lastClick.Y),
                    new System.Windows.Point(mainControl.OriginalImageCanvas.Width - 1, lastClick.Y), System.Windows.Media.Brushes.Red, 1);
                DrawHelper.DrawAndGetLine(mainControl.OriginalImageCanvas, new System.Windows.Point(lastClick.X, 0),
                    new System.Windows.Point(lastClick.X, mainControl.OriginalImageCanvas.Height - 1), System.Windows.Media.Brushes.Red, 1);
                DrawHelper.DrawAndGetRectangle(mainControl.OriginalImageCanvas, new System.Windows.Point(lastClick.X - 4, lastClick.Y - 4),
                    new System.Windows.Point(lastClick.X + 4, lastClick.Y + 4), System.Windows.Media.Brushes.Red);
                if (mainControl.ProcessedGrayscaleImage != null)
                {
                    DrawHelper.DrawAndGetLine(mainControl.ProcessedImageCanvas, new System.Windows.Point(0, lastClick.Y),
                    new System.Windows.Point(mainControl.ProcessedImageCanvas.Width - 1, lastClick.Y), System.Windows.Media.Brushes.Red, 1);
                    DrawHelper.DrawAndGetLine(mainControl.ProcessedImageCanvas, new System.Windows.Point(lastClick.X, 0),
                        new System.Windows.Point(lastClick.X, mainControl.ProcessedImageCanvas.Height - 1), System.Windows.Media.Brushes.Red, 1);
                    DrawHelper.DrawAndGetRectangle(mainControl.ProcessedImageCanvas, new System.Windows.Point(lastClick.X - 4, lastClick.Y - 4),
                        new System.Windows.Point(lastClick.X + 4, lastClick.Y + 4), System.Windows.Media.Brushes.Red);
                }
                if (mainControl.OriginalGrayscaleImage != null) MagnifWindow.RedrawMagnifyer(lastClick);
            }

            if (Selection.IsChecked)
            {
                MouseHelper.Point1 = lastClick;
                MouseHelper.IsSelecting = true;

                mainControl.OriginalImageCanvas.MouseUp += new MouseButtonEventHandler(picOriginal_MouseUp);
            }
        }

        private void openGrayscaleImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mainControl.LoadImageDialog(ImageType.Grayscale);
            Magnifyer_ON.IsEnabled = true;
            GL_ROW_ON.IsEnabled = true;
            Selection.IsEnabled = true;
            Histogram_ON.IsEnabled = true;
        }

        private void openColorImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mainControl.LoadImageDialog(ImageType.Color);
            Magnifyer_ON.IsEnabled = true;
            GL_ROW_ON.IsEnabled = true;
        }

        private void saveProcessedImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!mainControl.SaveProcessedImageToDisk())
            {
                MessageBox.Show("Processed image not available!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void saveAsOriginalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.ProcessedGrayscaleImage != null)
            {
                mainControl.OriginalGrayscaleImage = mainControl.ProcessedGrayscaleImage;
            }
            else if (mainControl.ProcessedColorImage != null)
            {
                mainControl.OriginalColorImage = mainControl.ProcessedColorImage;
            }
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                mainControl.ProcessedGrayscaleImage = Tools.Invert(mainControl.OriginalGrayscaleImage);
            }

        }

        private void Magnifyer_ON_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                if (Magif_SHOW == true)
                {
                    Magif_SHOW = false;
                    MagnifWindow.Close();
                    DrawHelper.RemoveAllLines(mainControl.OriginalImageCanvas);
                    DrawHelper.RemoveAllRectangles(mainControl.OriginalImageCanvas);
                    DrawHelper.RemoveAllLines(mainControl.ProcessedImageCanvas);
                    DrawHelper.RemoveAllRectangles(mainControl.ProcessedImageCanvas);

                }
                else Magif_SHOW = true;
                if (Magif_SHOW == true)
                {
                    MagnifWindow = new Windows.Magnifyer(mainControl.OriginalGrayscaleImage, mainControl.ProcessedGrayscaleImage);
                    MagnifWindow.Show();
                    MagnifWindow.RedrawMagnifyer(lastClick);
                }
            }
        }

        private void GL_ROW_ON_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                if (GL_ROW_SHOW == true)
                {
                    GL_ROW_SHOW = false;
                    RowDisplay.Close();
                    DrawHelper.RemoveAllLines(mainControl.OriginalImageCanvas);
                    DrawHelper.RemoveAllLines(mainControl.ProcessedImageCanvas);
                }
                else GL_ROW_SHOW = true;

                if (GL_ROW_SHOW == true)
                {
                    RowDisplay = new Windows.GLine(mainControl.OriginalGrayscaleImage, mainControl.ProcessedGrayscaleImage);

                    RowDisplay.Show();
                    RowDisplay.Redraw((int)lastClick.Y);

                }
            }
        }

        private void MirrorImage_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                mainControl.ProcessedGrayscaleImage = Tools.MirrorOnYAxis(mainControl.OriginalGrayscaleImage);
            }
        }

        private void BinaryImage_Click(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                mainControl.ProcessedGrayscaleImage = Tools.BinarizeImage(mainControl.OriginalGrayscaleImage, 127);
            }
        }

        private void picOriginal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!MouseHelper.IsSelecting)
                return;
            MouseHelper.IsSelecting = false;

            MouseHelper.Point2 = Mouse.GetPosition(mainControl.OriginalImageCanvas);

            System.Windows.Media.Brush brush = new SolidColorBrush(Colors.Red);
            DrawHelper.DrawAndGetRectangle(mainControl.OriginalImageCanvas,
                                           new System.Windows.Point(Math.Min(MouseHelper.Point1.X, MouseHelper.Point2.X), Math.Min(MouseHelper.Point1.Y, MouseHelper.Point2.Y)),
                                           new System.Windows.Point(Math.Max(MouseHelper.Point1.X, MouseHelper.Point2.X), Math.Max(MouseHelper.Point1.Y, MouseHelper.Point2.Y)),
                                           brush);

            int width = (int)Math.Abs(MouseHelper.Point2.X - MouseHelper.Point1.X);
            int height = (int)Math.Abs(MouseHelper.Point2.Y - MouseHelper.Point1.Y);

            MouseHelper.CorrectPoints(height, width);

            if ((width < 1) || (height < 1))
                return;

            mainControl.ProcessedGrayscaleImage = Tools.CropImage(mainControl.OriginalGrayscaleImage, MouseHelper.Point1, MouseHelper.Point2);
            MouseHelper.IsSelecting = false;
            mainControl.OriginalImageCanvas.MouseUp -= new MouseButtonEventHandler(picOriginal_MouseUp);

            MessageBox.Show("Abaterea medie patratica: " + Tools.AverageSquareDerivation(mainControl.ProcessedGrayscaleImage, width, height).ToString());

            DrawHelper.RemoveAllRectangles(mainControl.OriginalImageCanvas);
        }

        private void HISTOGRAM_ON_Click(object sender, RoutedEventArgs e)
        {
            if (Histogram_ON.IsChecked)
            {
                if (mainControl.ProcessedGrayscaleImage != null)
                {
                    if (Histogram_SHOW == true)
                    {
                        Histogram_SHOW = false;
                        HistogramDisplay.Close();
                        DrawHelper.RemoveAllLines(mainControl.OriginalImageCanvas);
                    }
                    else Histogram_SHOW = true;
                }
                else
                {
                    MessageBox.Show("There is no processed image to be compared to. Please apply a transformation before checking the histogram!");
                    Histogram_ON.IsChecked = false;
                }
                if (Histogram_SHOW == true)
                {
                    HistogramDisplay = new Windows.GHistogram(mainControl.OriginalGrayscaleImage, mainControl.ProcessedGrayscaleImage);
                    HistogramDisplay.Show();
                    HistogramDisplay.Redraw();
                }
            }
        }

        private void Click_GamaOperator(object sender, RoutedEventArgs e)
        {
            InputValue gammaInput = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                gammaInput.ShowDialog();
                    float gamma = gammaInput.GetValue();
                    if (gamma != 0)
                        mainControl.ProcessedGrayscaleImage = Tools.Gamma(mainControl.OriginalGrayscaleImage, gamma);
             }
             else
                {
                    MessageBox.Show("Error! There is no image to be processed.");
                }
        }

        private void Click_SinusOperator(object sender, RoutedEventArgs e)
        {
            mainControl.ProcessedGrayscaleImage = Tools.Sinus(mainControl.OriginalGrayscaleImage);
        }

        private void Click_OtsuBinarization(object sender, RoutedEventArgs e)
        {
            if (mainControl.OriginalGrayscaleImage != null)
            {
                double T = Tools.GetOtsuThreshold(mainControl.OriginalGrayscaleImage);
                mainControl.ProcessedGrayscaleImage = Tools.BinarizeImage(mainControl.OriginalGrayscaleImage, T);
            } else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }
        private void Click_GaussianOperator(object sender, RoutedEventArgs e)
        {
            InputValue gaussInput = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                gaussInput.ShowDialog();
                float sigma = gaussInput.GetValue();
                if (sigma != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.GaussianFilter(mainControl.OriginalGrayscaleImage, sigma);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_MedianFilter(object sender, RoutedEventArgs e)
        {
            InputValue maskInput = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                maskInput.ShowDialog();
                int maskDim = (int)maskInput.GetValue();
                if (maskDim != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.MedianFilter(mainControl.OriginalGrayscaleImage, maskDim);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_PrewittFilter(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };

            InputValue tValue = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                tValue.ShowDialog();
                int T = (int)tValue.GetValue();
                if (T != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.SimpleEdgeDetection(mainControl.OriginalGrayscaleImage, mask, T);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_SobelFilter(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

            InputValue tValue = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                tValue.ShowDialog();
                int T = (int)tValue.GetValue();
                if (T != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.SimpleEdgeDetection(mainControl.OriginalGrayscaleImage, mask, T);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_SobelDirectional(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

            InputValue tValue = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                tValue.ShowDialog();
                int T = (int)tValue.GetValue();
                if (T != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.SobelDirectional(mainControl.OriginalGrayscaleImage, mask, T);
                }

            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_RobertsFilter(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { -2, -1, 0 }, { -1, 0, 1 }, { 0, 1, 2 } };
            InputValue tValue = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                tValue.ShowDialog();
                int T = (int)tValue.GetValue();
                if (T != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.RobertsEdgeDetection(mainControl.OriginalGrayscaleImage, mask, T);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }

        private void Click_OpeningMorphology(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            if (mainControl.OriginalGrayscaleImage != null)
                mainControl.ProcessedGrayscaleImage = Tools.OpeningMorphology(mainControl.OriginalGrayscaleImage, mask);
            else
                MessageBox.Show("Error! There is no image to be processed.");
        }

        private void Click_ClosingMorphology(object sender, RoutedEventArgs e)
        {
            int[,] mask = new int[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            if (mainControl.OriginalGrayscaleImage != null)
                mainControl.ProcessedGrayscaleImage = Tools.ClosingMorphology(mainControl.OriginalGrayscaleImage, mask);
            else
                MessageBox.Show("Error! There is no image to be processed.");
        }

        private void Click_BilinearInterpolation(object sender, RoutedEventArgs e)
        {
            InputValue tValue = new InputValue();
            if (mainControl.OriginalGrayscaleImage != null)
            {
                tValue.ShowDialog();
                int coef = (int)tValue.GetValue();
                if (coef != 0)
                {
                    mainControl.ProcessedGrayscaleImage = Tools.ScaleWithBilinearInterpolation(mainControl.OriginalGrayscaleImage, coef);
                }
            }
            else
            {
                MessageBox.Show("Error! There is no image to be processed.");
            }
        }
    }
}
