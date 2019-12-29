
using System;
using System.Windows;

namespace ISIP_Algorithms.Help
{
    public class MouseHelper
    {
        public static bool IsSelecting { get; set; } = false;
        public static Point Point1 { get; set; }
        public static Point Point2 { get; set; }
        
        private static void AbsoluteCoordinates()
        {
            Point1 = new Point(Math.Abs(Point1.X), Math.Abs(Point1.Y));
            Point2 = new Point(Math.Abs(Point2.X), Math.Abs(Point2.Y));
        }

        public static void CorrectPoints(double height, double width)
        {
            AbsoluteCoordinates();
            if (Point1.X > Point2.X && Point1.Y > Point2.Y)
            {
                Point aux = Point1;
                Point1 = Point2;
                Point2 = aux;
                return;
            }

            if(Point1.X < Point2.X && Point1.Y > Point2.Y)
            {
                double newPoint1Y = Point1.Y - height;
                double newPoint2Y = Point2.Y + height; ;
                Point1 = new Point(Point1.X, newPoint1Y);
                Point2 = new Point(Point2.X, newPoint2Y);
            }
            if (Point1.X > Point2.X && Point1.Y < Point2.Y)
            {
                double newPoint1X = Point1.X - width;
                double newPoint2X = Point2.X + width;
                Point1 = new Point(newPoint1X, Point1.Y);
                Point2 = new Point(newPoint2X, Point2.Y);
            }
            
        }
    }
}
