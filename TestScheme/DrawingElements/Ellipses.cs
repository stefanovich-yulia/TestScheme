using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestScheme.DrawingElements
{
    class Ellipses: Shapes
    {
        public Ellipses() : base(){}

        public Ellipses(Brush brush) : base(brush) { }
        public Ellipses(Point locationPointStart, Point locationPointEnd = default(Point))
            : base(locationPointStart, locationPointEnd) { }
        public Ellipses(Brush brush, Point locationPointStart, Point locationPointEnd = default(Point))
            : base(brush, locationPointStart, locationPointEnd) { }

        public override void Draw(Canvas paintSurface)
        {
            Ellipse elps = new Ellipse
            {
                Width = Radius,
                Height = Radius,
                Fill = Brush
            };
            paintSurface.Children.Add(elps);

            Canvas.SetLeft(elps, LocationPoint.X);
            Canvas.SetTop(elps, LocationPoint.Y);
        }
    }
}
