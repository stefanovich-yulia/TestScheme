using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TestScheme.Schemes.Objects.Elements;

namespace TestScheme.DrawingElements
{
    class Rectangles: Shapes
    {
        public Rectangles( ) : base() { }
        public Rectangles(Brush brush) : base(brush) { }
        public Rectangles(Point locationPointStart, Point locationPointEnd = default(Point))
            : base(locationPointStart, locationPointEnd) { }
        public Rectangles(Brush brush, Point locationPointStart, Point locationPointEnd = default(Point)) 
            : base(brush, locationPointStart, locationPointEnd) { }



        public override void Draw(Canvas paintSurface)
        {
            Rectangle rect = new Rectangle
            {
                Width = Width,
                Height = Height
            };
            if (Brush.Equals(Element.ChoosedElementBrush))
            {
                rect.Stroke = Brush;
                rect.StrokeThickness = 2;
            }
            else
                rect.Fill = Brush;

            paintSurface.Children.Add(rect);
            Canvas.SetLeft(rect, LocationPoint.X);
            Canvas.SetTop(rect, LocationPoint.Y);
        }
    }
}
