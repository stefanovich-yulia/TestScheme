
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestScheme.DrawingElements
{
    public abstract class Shapes
    {
        private const double R = 5 * 1.5; 
        private const double W = 50 * 1.5;
        private const double H = 25 * 1.5;

        public static double Width => W;

        public static double Height => H;

        public static double Radius => R;

        public Brush Brush { get; set; }

        public Point LocationPoint { get; set; }

        protected Shapes()
        {
            this.Brush = Brushes.Black;
        }
        protected Shapes(Brush brush)
        {
            this.Brush = brush;
        }
        protected Shapes(Point locationPointStart, Point locationPointEnd = default(Point)) : this()
        {
            this.LocationPoint = locationPointStart;
        }
        protected Shapes(Brush brush, Point locationPointStart, Point locationPointEnd = default(Point)) : this()
        {
            this.Brush = brush;
            this.LocationPoint = locationPointStart;
        }

        public abstract void Draw(Canvas paintSurface);
    }
}
