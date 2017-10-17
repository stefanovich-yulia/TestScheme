using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using TestScheme.Scheme;
using TestScheme.Scheme.Objects;

namespace TestScheme
{
    //public interface IObjects
    //{
    //    void SetLocationPoints(List<Point> points);
    //    List<Point> GetLocationPoints();

    //    void Draw(Canvas paintSurface);
    //    void DrawChoosedElem(Canvas paintSurface);
    //    bool IsSelected();
    //    int CheckLocation(Point checkingPoint, int var);
    //    void Drag(int newX, int newY);

    //}

    public abstract class Element // : IObjects
    {
        #region Const, properties
        Flow _flow;

        const double _w = 50;
        const double _h = 25;
        const double _r = 5;
        readonly SolidColorBrush _connectingEllipseBrush = Brushes.Black;
        readonly SolidColorBrush _selectedConnectingEllipseBrush = Brushes.Red;
        #region width, height, radius, color, selected
        protected double Width
        {
            get => _w;
        }
        protected double Height
        {
            get => _h;
        }
        protected double Radius
        {
            get => _r;
        }
        protected Color ElemColor
        {
            get; set;
        }
        protected SolidColorBrush GetConnectingEllipseBrush()
        {
            return _connectingEllipseBrush;
        }
        public SolidColorBrush GetSelectedConnectingEllipseBrush()
        {
            return _selectedConnectingEllipseBrush;
        }
        public bool Selected
        {
            get; set;
        }
        #endregion

        #region input, output
        public Vertex OutElement // элемент к которому идет выход (эл-т со входом и номер его входа)
        {
            get; set;
        }
        public List<Element> InputElements // элементы (выходы) которые соединены с входами данного эл-та
        {
            get; set;
        }

        static public void CheckElemCountInList(List<Element> list, int numb)
        {
            if (list.Count < numb + 1)
                for (int i = 0; i <= numb; i ++)
                    list.Add(null);
        }
        public Point OutPoint // точка - вход эл-та
        {
            get; set;
        }
        public List<Point> InputPoints
        {
            get; set;
        } 
        #endregion

        #region locationPoint

        private Point _locationPoint;
        public Point LocationPoint { get => _locationPoint; set => _locationPoint = value; }

        #endregion

        #endregion

        #region Draw
        public void Draw(Canvas paintSurface)
        {
            Rectangle rect = DrawRectangle(paintSurface);
            rect.Fill = new SolidColorBrush(ElemColor);
            DrawInOut(paintSurface);

        }
        public void DrawChoosedElem(Canvas paintSurface)
        {
            Rectangle rect = DrawRectangle(paintSurface);
            rect.Stroke = Brushes.Green;
            rect.StrokeThickness = 2;

        }

        public void DrawConnections(Canvas paintSurface)
        {
            if (this.OutElement != null & this.OutPoint != default(Point))
            {
                Lines.Draw(paintSurface, this.OutPoint, this.OutElement.elem.InputPoints[this.OutElement.indexInList]);
            }
        }
        public abstract void DrawInOut(Canvas paintSurface);

        protected Rectangle DrawRectangle(Canvas paintSurface)
        {
            Rectangle rect = new Rectangle();
            rect.Width = this.Width;
            rect.Height = this.Height;
            paintSurface.Children.Add(rect);
            Canvas.SetLeft(rect, this.LocationPoint.X);
            Canvas.SetTop(rect, this.LocationPoint.Y);
            return rect;
        }
        public Ellipse DrawEllipse(Canvas paintSurface, Point location, SolidColorBrush brush)
        {
            Ellipse elps = new Ellipse();
            elps.Width = this.Radius;
            elps.Height = this.Radius;
            elps.Fill = brush;
            paintSurface.Children.Add(elps);

            Canvas.SetLeft(elps, location.X);
            Canvas.SetTop(elps, location.Y);

            return elps;
        }
        #endregion

        #region CheckLocation
        bool CheckPointBelongElement (Point locationPoint, Point checkingPoint, double dxRight, double dxLeft, double dyTop, double dyBottom)
        {
            if (checkingPoint.X >= locationPoint.X - dxLeft &
                checkingPoint.X <= locationPoint.X + dxRight &
                checkingPoint.Y >= locationPoint.Y - dyTop &
                checkingPoint.Y <= locationPoint.Y + dyBottom)
                return true;
            else
                return false;
        }

        public bool CheckSelection(Point checkingPoint)
        {
            if (CheckPointBelongElement(this.LocationPoint, checkingPoint, this.Width, 0, 0, this.Height))
                return true;
            else
                return false;
        }

        public bool CheckOutput(Point checkingPoint)
        {

            if (this.OutPoint != default(Point)) // у эл-та предусмотрен выход
            {
                if (CheckPointBelongElement(this.OutPoint, checkingPoint, this.Width, 0, 0, this.Height))
                    if (this.OutElement != null)
                    {
                        MessageBox.Show("У данного элемента уже есть выходное соединение");
                        return false;
                    }
                    else
                        return true;
                else
                    return false;
            }

            else
                return false;
        }


        public bool CheckInput(Point checkingPoint, out Vertex vertex)
        {
            if (this.InputPoints != null) // у эл-та предусмотрен вход (несколько)
            {
                foreach (Point inPoint in this.InputPoints)
                    if (CheckPointBelongElement(inPoint, checkingPoint, this.Radius, this.Radius, this.Radius, this.Radius))
                    {
                        vertex = new Vertex(this, this.InputPoints.IndexOf(inPoint));
                        return true;
                        //break;
                    }
                vertex = null;
                return false;
                    
            }
            else
            {
                vertex = null;
                return false;
            }

        }



        #endregion

        public void Drag(double dX, double dY)
        {
            this._locationPoint.X = this._locationPoint.X + dX;
            this._locationPoint.Y = this._locationPoint.Y + dY;
        }

        public void RemoveElem()
        {
            int indexInScheme = Scheme.SchemeElements.IndexOf(this);
            if (this.OutElement != null)
            {
                int indexInInputsList = this.OutElement.indexInList;
                if (Scheme.SchemeElements[indexInScheme].OutElement.elem.InputElements.Count >=
                    indexInInputsList)
                    Scheme.SchemeElements[indexInScheme].OutElement.elem.InputElements[indexInInputsList] =
                        null;
            }
            if (this.InputElements != null)
            {
                foreach (Element elem in this.InputElements)
                    elem.OutElement = null;
            }
            Scheme.SchemeElements.Find(elem => elem == this).OutElement = null;
            Scheme.SchemeElements.Remove(this);
        }
    }

    public class Source: Element
    {
        // св-ва

        #region конструктор
        public Source()
        {
            ElemColor = Color.FromRgb(7, 199, 255);
        }

        #endregion
        public override void DrawInOut(Canvas paintSurface)
        {
            double x = this.LocationPoint.X + this.Width - this.Radius / 2;
            double y = this.LocationPoint.Y + this.Height / 2 - this.Radius / 2;
            this.OutPoint = new Point(x, y);

            DrawEllipse(paintSurface, this.OutPoint, GetConnectingEllipseBrush());
        }
    }
    public class Pipe: Element
    {
        // св-ва
        public double Asperity { get; set; }  // шероховатость
        public double Length { get; set; }
        public double Diameter { get; set; }
        #region конструктор
        public Pipe()
        {
            ElemColor = Color.FromRgb(109, 110, 96);
            InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }
        #endregion

        public override void DrawInOut(Canvas paintSurface)
        {
            double x = this.LocationPoint.X + this.Width - (double)Radius / 2;
            double y = this.LocationPoint.Y + this.Height / 2 - (double)Radius / 2;
            this.OutPoint = new Point(x, y);

            this.InputPoints.Clear();
            x = this.LocationPoint.X - (double)Radius / 2;
            this.InputPoints.Add(new Point(x, y));

            DrawEllipse(paintSurface, this.OutPoint, GetConnectingEllipseBrush());
            DrawEllipse(paintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
        }
    }
    public class HeatExchanger : Element
    {
        // св-ва
        #region конструктор
        public HeatExchanger()
        {
            ElemColor = Color.FromRgb(255, 15, 48);
            InputPoints = new List<Point>();
            InputElements = new List<Element>(2);
        }

        #endregion


        public override void DrawInOut(Canvas paintSurface)
        {
            double x = LocationPoint.X + this.Width - Radius / 2;
            double y = LocationPoint.Y + this.Height / 2 - Radius / 2;
            this.OutPoint = new Point(x, y);

            this.InputPoints.Clear();
            x = LocationPoint.X - Radius / 2;
            y = LocationPoint.Y + Radius;
            this.InputPoints.Add(new Point(x, y));
            y = LocationPoint.Y + this.Height - 2 * Radius;
            this.InputPoints.Add(new Point(x, y));

            DrawEllipse(paintSurface, this.OutPoint, GetConnectingEllipseBrush());
            DrawEllipse(paintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
            DrawEllipse(paintSurface, this.InputPoints[1], GetConnectingEllipseBrush());
        }
    }
    public class Flowing : Element
    {
        // св-ва
        #region конструктор
        public Flowing()
        {
            ElemColor = Color.FromRgb(227, 228, 209);
            InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }

        #endregion

        public override void DrawInOut(Canvas paintSurface)
        {
            this.InputPoints.Clear();
            double x = LocationPoint.X - Radius / 2;
            double y = LocationPoint.Y + this.Height / 2 - Radius / 2;
            this.InputPoints.Add(new Point(x, y));

            DrawEllipse(paintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
        }
    }


    //public abstract class Lines : IObjects
    //{
    //    #region points
    //    Point _pointStart, _pointEnd;
    //    Element _elemStart, _elemEnd;

    //    public void SetLocationPoints(List<Point> points)
    //    {
    //        if (points != null & points.Count >= 2)
    //        {
    //            _pointStart = points[0];
    //            _pointEnd = points[1];
    //        }
    //    }
    //    public void SetPointStart(Element elem)
    //    {
    //        this._pointStart = elem.OutPoint;
    //    }
    //    public void SetPointEnd(Element elem, int i)
    //    {
    //        this._pointEnd = elem.InputPoints[i];
    //    }

    //    public List<Point> GetLocationPoints()
    //    {
    //        return new List<Point> { _pointStart, _pointEnd };
    //    }

    //    #endregion

    //    public void Draw(Canvas paintSurface)
    //    {
    //        Line line = new Line();
    //        line.X1 = _pointStart.X;
    //        line.Y1 = _pointStart.Y;
    //        line.X2 = _pointEnd.X;
    //        line.Y2 = _pointEnd.Y;

    //        line.Stroke = System.Windows.Media.Brushes.Black;

    //        //line.HorizontalAlignment = HorizontalAlignment.Left;
    //        //line.VerticalAlignment = VerticalAlignment.Center;
    //        line.StrokeThickness = 2;
    //        paintSurface.Children.Add(line);

    //    }
    //    public void DrawChoosedElem(Canvas paintSurface)
    //    {

    //    }
    //    public int CheckLocation(Point checkingPoint, int var)
    //    {
    //        return 0;
    //    }
    //    public bool IsSelected()
    //    {
    //        return false;
    //    }
    //    public void Drag(int newX, int newY)
    //    {
    //    }
    //}

    //public class ConnectingLine : Lines
    //{


    //}

    //public class InOut
    //{
    //    Element elem; // ссылка на элемент, с кот соединяется
    //    Ellipse elps; // отображение вх/выхода

    //}

}
