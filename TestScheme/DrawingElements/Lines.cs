using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestScheme.DrawingElements
{
    public class Lines: Shapes
    {
        public Point LocationPointEnd { get; set; }

        public Lines( ) : base(){ }
        public Lines(Brush brush) : base(brush){ }

        public Lines(Point locationPointStart, Point locationPointEnd = default(Point))
            : base(locationPointStart, locationPointEnd)
        {
            LocationPointEnd = locationPointEnd;
        }

        public Lines(Brush brush, Point locationPointStart, Point locationPointEnd = default(Point))
            : base(brush, locationPointStart, locationPointEnd) 
        {
            LocationPointEnd = locationPointEnd;
        }

        public override void Draw(Canvas paintSurface)
        {
            
            Line line = new Line
            {
                X1 = LocationPoint.X + Radius / 2,
                Y1 = LocationPoint.Y + Radius / 2,
                X2 = LocationPointEnd.X + Radius / 2,
                Y2 = LocationPointEnd.Y + Radius / 2,

                Stroke = Brushes.Black,

                StrokeThickness = 2
            };
            paintSurface.Children.Add(line);

        }

        //public static void CheckLine
    }
}
