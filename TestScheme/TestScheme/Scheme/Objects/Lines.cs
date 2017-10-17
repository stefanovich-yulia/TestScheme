using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TestScheme.Scheme.Objects
{
    public static class Lines
    {

        public static void Draw(Canvas paintSurface, Point pointStart, Point pointEnd)
        {
            Line line = new Line();
            line.X1 = pointStart.X;
            line.Y1 = pointStart.Y;
            line.X2 = pointEnd.X;
            line.Y2 = pointEnd.Y;

            line.Stroke = System.Windows.Media.Brushes.Black;

            line.StrokeThickness = 2;
            paintSurface.Children.Add(line);

        }
    }
}
