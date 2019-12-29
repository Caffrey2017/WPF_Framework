using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows;

namespace ISIP_Algorithms.Tools
{
    public class Tools
    {
        public static Image<Gray, byte> Invert(Image<Gray, byte> InputImage)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    Result.Data[y, x, 0] = (byte)(255 - InputImage.Data[y, x, 0]);
                }
            }
            return Result;
        }

        public static Image<Gray, byte> MirrorOnYAxis(Image<Gray, byte> InputImage)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = InputImage.Width - 1; x >= 0; x--)
                {
                    Result.Data[y, InputImage.Width - x - 1, 0] = (byte)(InputImage.Data[y, x, 0]);
                }
            }
            return Result;
        }

        public static Image<Gray, byte> BinarizeImage(Image<Gray, byte> InputImage, double threshold)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    if ((byte)(InputImage.Data[y, x, 0]) < threshold)
                    {
                        Result.Data[y, x, 0] = 0;
                    }
                    else
                    {
                        Result.Data[y, x, 0] = 255;
                    }
                }
            }
            return Result;
        }

        public static Image<Gray, byte> CropImage(Image<Gray, byte> InputImage, Point Point1, Point Point2)
        {
            int width = Math.Abs((int)Point2.X - (int)Point1.X);
            int height = (int)Math.Abs((int)Point2.Y - (int)Point1.Y);

            Image<Gray, byte> Result = new Image<Gray, byte>(new System.Drawing.Size(width + 1, height + 1));

            int startX = (int)Point1.X;
            int startY = (int)Point1.Y;
            int endX = (int)Point2.X;
            int endY = (int)Point2.Y;
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    Result.Data[y - startY, x - startX, 0] = InputImage.Data[y, x, 0];
                }
            }
            return Result;
        }

        public static double AverageSquareDerivation(Image<Gray, byte> InputImage, int width, int height)
        {
            double u = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    u += InputImage.Data[y, x, 0];
                }
            }

            u /= (height * width);

            double sigma2 = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sigma2 += (InputImage.Data[y, x, 0] - u) * (InputImage.Data[y, x, 0] - u);
                }
            }
            sigma2 /= (height * width);

            return sigma2;
        }
        public static int[] GenerateHistogram(Image<Gray, byte> InputImage)
        {
            int[] greyLevels = new int[256];
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    greyLevels[(byte)InputImage.Data[y, x, 0]]++;
                }
            }
            return greyLevels;
        }

        public static double[] GetHistogramEqualization(Image<Gray, byte> InputImage)
        {
            double[] equalization = new double[256];
            int[] histogram = GenerateHistogram(InputImage);
            double total = InputImage.Height * InputImage.Width;
            for (int i = 0; i <= 255; i++)
            {
                equalization[i] = histogram[i] / total;
            }
            return equalization;
        }

        public static Image<Gray, byte> Gamma(Image<Gray, byte> InputImage, float gamma)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            double a = 255 / Math.Pow(255, gamma);

            float r = 0;
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    r = InputImage.Data[y, x, 0]; // pixel
                    Result.Data[y, x, 0] = (byte)(a * Math.Pow(r, gamma));
                }
            }
            return Result;
        }

        public static Image<Gray, byte> Sinus(Image<Gray, byte> InputImage)
        {
            const double alpha = 255 / 2; //ca sa transpuna valorile din [0, 2] in valori intre 0 si 255
            const double beta = Math.PI / 255; //da noua nuanta
            const double gamma = -Math.PI / 2;  //pentru shiftarea sinusoidei pe pozitiv 

            double[] lut = new double[256];
            for (int r = 0; r < 256; r++)
                lut[r] = alpha * (Math.Sin(beta * r + gamma) + 1);

            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    int pixel = (int)(lut[InputImage.Data[y, x, 0]]);
                    Result.Data[y, x, 0] = (byte)pixel;
                }
            }
            return Result;
        }

        public static double GetOtsuThreshold(Image<Gray, byte> InputImage)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            int[] histogram = new int[256];
            double[] histogramEqualization = new double[256];
            histogram = GenerateHistogram(InputImage);
            histogramEqualization = GetHistogramEqualization(InputImage);

            // T0
            double P1, P2, miu1 = 0, miu2 = 0;
            P1 = histogramEqualization[0];
            P2 = 1 - P1;
            for (int k = 1; k < 256; k++)
                miu2 += k * histogramEqualization[k];

            double sigma2B = 0;
            double max = 0;
            int sum = 0;
            int nr = 1;
            
            for(int t = 1; t < 256; t++)
            {
                P1 += histogramEqualization[t];
                P2 -= histogramEqualization[t];

                miu1 += t * histogramEqualization[t];
                miu2 -= t * histogramEqualization[t];

                if (P1 == 0 || P2 == 0)
                    sigma2B = 0;
                else
                    sigma2B = P1 * P2 * ((miu2 / P2) - (miu1 / P1)) * ((miu2 / P2) - (miu1 / P1));

                if(sigma2B > max)
                {
                    max = sigma2B;
                    sum = t;
                    nr = 1;
                }

                if(sigma2B == max)
                {
                    sum += t;
                    nr++;
                }
            }

            double average = sum / nr;
            return average;
        }

        public static Image<Gray, byte> GaussianFilter(Image<Gray, byte> InputImage, float sigma)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            Result = InputImage.Clone();

            int k = (int)(4 * sigma + 1);
            double[,] mask = new double[k, k];

            double sum = 0;
            for (int i = -k / 2; i <= k / 2; i++)
            {
                for (int j = -k / 2; j <= k / 2; j++)
                {
                    mask[i + k / 2, j + k / 2] = Math.Exp(-((i * i + j * j) / (2 * sigma * sigma)));
                    sum += mask[i + k / 2, j + k / 2];
                }
            }

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    mask[i, j] /= sum;
                }
            }

            for (int y = 0; y < InputImage.Height - k; y++)
            {
                for (int x = 0; x < InputImage.Width - k; x++)
                {
                    double sumMatrix = 0;
                    for (int i = 0; i < k; i++)
                        for (int j = 0; j < k; j++)
                        {
                            if ((y + i) < (InputImage.Width) && (x + j) < (InputImage.Height))
                                sumMatrix += mask[i, j] * InputImage.Data[y + i, x + j, 0];
                        }
                    if ((y + (k / 2)) < (InputImage.Height - (k / 2)) && (x + (k / 2)) < (InputImage.Width - (k / 2)))
                        Result.Data[y + (k / 2), x + (k / 2), 0] = (byte)sumMatrix;
                }
            }
            return Result;
        }

        private static byte[] SortMask(byte[] mask)
        {
            Array.Sort(mask);
            return mask;
        }
        private static byte[] GetMask(Image<Gray, byte> InputImage, int x, int y, int maskDim) // img, pixel centru, dim masca
        {
            byte[] mask = new byte[maskDim * maskDim];
            int dim = 0;
            for (int i = y - maskDim / 2; i <= y + maskDim / 2; i++)
            {
                for (int j = x - maskDim / 2; j <= x + maskDim / 2; j++)
                {
                    mask[dim++] = InputImage.Data[i, j, 0];
                }
            }
            return mask;
        }
        public static Image<Gray, byte> MedianFilter(Image<Gray, byte> InputImage, int maskDim)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            //Result = InputImage.Clone();
            byte[] mask = new byte[maskDim * maskDim];
            byte[] sortedMask = new byte[maskDim * maskDim];
            for (int y = maskDim / 2; y < InputImage.Height - maskDim / 2; y++)
            {
                for (int x = maskDim / 2; x < InputImage.Width - maskDim / 2; x++)
                {
                    mask = GetMask(InputImage, x, y, maskDim);
                    sortedMask = SortMask(mask);
                    Result.Data[y, x, 0] = sortedMask[(maskDim * maskDim) / 2 + 1];
                }
            }
            return Result;
        }

        public static int[,] MatrixBackTranspose(int[,] matrix)
        {
            int[,] tMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)]; // 3x3 de fapt
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    tMatrix[j, i] = matrix[i, j];
            return tMatrix;
        }

        public static int GetAreaResult(Image<Gray, byte> InputImage, int[,] mask, int x, int y)
        {
            int result = 0;

            result += InputImage.Data[y - 1, x - 1, 0] * mask[0, 0];
            result += InputImage.Data[y - 1, x, 0] * mask[0, 1];
            result += InputImage.Data[y - 1, x + 1, 0] * mask[0, 2];
            result += InputImage.Data[y, x - 1, 0] * mask[1, 0];
            result += InputImage.Data[y, x, 0] * mask[1, 1];
            result += InputImage.Data[y, x + 1, 0] * mask[1, 2];
            result += InputImage.Data[y + 1, x - 1, 0] * mask[2, 0];
            result += InputImage.Data[y + 1, x, 0] * mask[2, 1];
            result += InputImage.Data[y + 1, x + 1, 0] * mask[2, 2];

            return result;
        }
        
        public static Image<Gray, byte> SimpleEdgeDetection(Image<Gray, byte> InputImage, int[,] sobelX, int T)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            int[,] sobelY = new int[3, 3];
            sobelY = MatrixBackTranspose(sobelX);

            int Px = 0, Py = 0;
            for (int y = 1; y < InputImage.Height - 1; y++)
            {
                for (int x = 1; x < InputImage.Width - 1; x++)
                {
                    Px = GetAreaResult(InputImage, sobelX, x, y);
                    Py = GetAreaResult(InputImage, sobelY, x, y);

                    double value = Math.Sqrt((Px * Px) + (Py * Py));
                    
                   if (Math.Ceiling(value) > T)
                        Result.Data[y, x, 0] = 255;
                    else
                        Result.Data[y, x, 0] = 0;
                }
            }

            return Result;
        }

        public static Image<Gray, byte> SobelDirectional(Image<Gray, byte> InputImage, int[,] sobelX, double T)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            int[,] sobelY = new int[3, 3];
            sobelY = MatrixBackTranspose(sobelX);

            int Px = 0, Py = 0;
            const double rad = 180/Math.PI;
            for (int y = 1; y < InputImage.Height - 1; y++)
            {
                for (int x = 1; x < InputImage.Width - 1; x++)
                {
                    Px = GetAreaResult(InputImage, sobelX, x, y);
                    Py = GetAreaResult(InputImage, sobelY, x, y);

                    double angle = Math.Atan2(Py, Px);
                    if (Math.Sqrt(Px * Px + Py * Py) >= T)
                    {
                        double degrees = angle * rad;
                        if (degrees >= -7 && degrees <= 7)
                            Result.Data[y, x, 0] = 255;
                        else
                            Result.Data[y, x, 0] = 0;
                    }
                }
            }

            return Result;
        }

        public static Image<Gray, byte> RobertsEdgeDetection(Image<Gray, byte> InputImage, int[,] robertsX, int T)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            int[,] robertsY = new int[3, 3];
            Array.Copy(robertsX, robertsY, robertsX.Length);
            
                for (int i = 0; i < 3; i++)
                {
                    int aux = robertsY[i, 0];
                    robertsY[i, 0] = robertsY[i, 2];
                    robertsY[i, 2] = aux;
                }

            int Rx = 0, Ry = 0;

            for (int y = 1; y < InputImage.Height - 1; y++)
            {
                for (int x = 1; x < InputImage.Width - 1; x++)
                {
                    Rx = GetAreaResult(InputImage, robertsX, x, y);
                    Ry = GetAreaResult(InputImage, robertsY, x, y);

                    double value = Math.Sqrt((Rx * Rx) + (Ry * Ry));
                    if(Math.Ceiling(value) > T)
                        Result.Data[y, x, 0] = 255;
                    else
                        Result.Data[y, x, 0] = 0;
                }
            }

            return Result;
        }

        private static bool FitsMask(Image<Gray, byte> InputImage, int[,] mask, int i, int j)
        {
            int yInit = i - mask.GetLength(0) / 2;
            int xInit = j - mask.GetLength(1) / 2;
            for(int y = yInit; y < i + mask.GetLength(0) /2; y++)
            {
                for(int x = xInit; x < j + mask.GetLength(1) /2; x++)
                {
                    if (InputImage.Data[y, x, 0] != mask[y - yInit, x - xInit])
                        return false;
                }
            }
            return true;
        }

        private static bool HitsMask(Image<Gray, byte> InputImage, int[,] mask, int i, int j)
        {
            int yInit = i - mask.GetLength(0) / 2;
            int xInit = j - mask.GetLength(1) / 2;
            for (int y = yInit; y < i + mask.GetLength(0) / 2; y++)
            {
                for (int x = xInit; x < j + mask.GetLength(1) / 2; x++)
                {
                    if (InputImage.Data[y, x, 0] == mask[y - yInit, x - xInit])
                        return true;
                }
            }
            return false;
        }

        public static Image<Gray, byte> OpeningMorphology(Image<Gray, byte> InputImage, int[,] mask)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);
            Result = InputImage.Clone();

            // eroziune
            for (int y = mask.GetLength(0) / 2; y < InputImage.Height - mask.GetLength(0) / 2; y++)
            {
                for (int x = mask.GetLength(1) / 2; x < InputImage.Width - mask.GetLength(1) / 2; x++)
                {
                    if (FitsMask(InputImage, mask, y, x))
                        Result.Data[y, x, 0] = 0;
                    else
                        Result.Data[y, x, 0] = 255;
                }
            }

            // dilatare
            for (int y = mask.GetLength(0) / 2; y < InputImage.Height - mask.GetLength(0) / 2; y++)
            {
                for (int x = mask.GetLength(1) / 2; x < InputImage.Width - mask.GetLength(1) / 2; x++)
                {
                    if (HitsMask(InputImage, mask, y, x))
                        Result.Data[y, x, 0] = 0;
                    else
                        Result.Data[y, x, 0] = 255;
                }
            }

            return Result;
        }

        public static Image<Gray, byte> ClosingMorphology(Image<Gray, byte> InputImage, int[,] mask)
        {
            Image<Gray, byte> Result = new Image<Gray, byte>(InputImage.Size);

            // dilatare
            for (int y = mask.GetLength(0) / 2; y < InputImage.Height - mask.GetLength(0) / 2; y++)
            {
                for (int x = mask.GetLength(1) / 2; x < InputImage.Width - mask.GetLength(1) / 2; x++)
                {
                    if (HitsMask(InputImage, mask, y, x))
                        Result.Data[y, x, 0] = 0;
                    else
                        Result.Data[y, x, 0] = 255;
                }
            }

            // eroziune
            for (int y = mask.GetLength(0) / 2; y < InputImage.Height - mask.GetLength(0) / 2; y++)
            {
                for (int x = mask.GetLength(1) / 2; x < InputImage.Width - mask.GetLength(1) / 2; x++)
                {
                    if (FitsMask(InputImage, mask, y, x))
                        Result.Data[y, x, 0] = 0;
                    else
                        Result.Data[y, x, 0] = 255;
                }
            }

            return Result;
        }

        public static Image<Gray, byte> ScaleWithBilinearInterpolation(Image<Gray, byte> InputImage, double coef)
        {
            int newWidth = (int)(InputImage.Width * coef);
            int newHeight = (int)(InputImage.Height * coef);
            Image<Gray, byte> Result = new Image<Gray, byte>(newWidth, newHeight);

            for (int y = 0; y < InputImage.Height; y++)
            {
                for (int x = 0; x < InputImage.Width; x++)
                {
                    int newy = (int)(y * coef);
                    int newx = (int)(x * coef);
                    Result.Data[newy, newx, 0] = InputImage.Data[y, x, 0];
                }
            }

            double xc, yc;
            int x0, y0;
            for (int y = 0; y < Result.Height; y++)
            {
                for (int x = 0; x < Result.Width; x++)
                {
                    xc = x / coef;
                    yc = y / coef;

                    x0 = (int)xc; // parte intreaga
                    y0 = (int)yc;

                    if (x0 + 1 >= InputImage.Width)
                        x0 = (int)(xc - 1);

                    if (y0 + 1 >= InputImage.Width)
                        y0 = (int)(yc - 1);

                    int pValue1 = InputImage.Data[y0, x0 + 1, 0];
                    int pValue2 = InputImage.Data[y0, x0, 0];

                    int value1 = (int)((pValue1 - pValue2) * (xc - x0) + pValue2);

                    int pValue3 = InputImage.Data[y0 + 1, x0 + 1, 0];
                    int pValue4 = InputImage.Data[y0 + 1, x0, 0];

                    int value2 = (int)((pValue3 - pValue4) * (xc - x0) + pValue4);

                    int pixel = (int)((value2 - value1) * (yc - y0) + value1);

                    Result.Data[y, x, 0] = (byte)pixel;

                    /*x1 = x0 + 1;
                    y1 = y0 + 1;
                    
                    double f0 = (InputImage.Data[y0, x1, 0] - InputImage.Data[y0, x0, 0]) / (x1 - x0) * (x - x0) + InputImage.Data[y0, x0, 0]; // f(x, y0)
                    double f1 = (InputImage.Data[y1, x1, 0] - InputImage.Data[y1, x0, 0]) /(x - x0) * (x - x0) + InputImage.Data[y1, x0, 0]; // f(x, y1)

                    double f2 = (f1 - f0) / (y1 - y0) * (y - y0) + Result.Data[y0, x, 0]; // f(x, y)

                    double f3 = (InputImage.Data[y0, x0 + 1, 0] - InputImage.Data[y0, x0, 0]) * (xc - x0) + InputImage.Data[y0, x0, 0]; // f(xc, y0)
                    double f4 = (InputImage.Data[y0 + 1, x0 + 1, 0] - InputImage.Data[y0 + 1, x0, 0]) * (xc - x0) + InputImage.Data[y0 + 1, x0, 0]; // f(xc, y0+1)

                    double fcc = (f4 - f3) * (yc - y0) + f3;

                    //int value = (int)((f1 - f2) * (yc - y0) + f2);

                    Result.Data[y, x, 0] = (byte)fcc;*/
                }
            }

            return Result;
        }

    }
}
