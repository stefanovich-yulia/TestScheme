using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TestScheme.Schemes.Objects
{
    public static class Lines
    {
        private const double R = 5;
        public static void Draw(Canvas PaintSurface, Point pointStart, Point pointEnd)
        {
            Line line = new Line();
            line.X1 = pointStart.X + R/2;
            line.Y1 = pointStart.Y + R / 2;
            line.X2 = pointEnd.X + R / 2;
            line.Y2 = pointEnd.Y + R / 2;

            line.Stroke = System.Windows.Media.Brushes.Black;

            line.StrokeThickness = 2;
            PaintSurface.Children.Add(line);

        }
    }
}
