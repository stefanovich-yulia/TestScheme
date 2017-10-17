using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestScheme.Schemes.Objects.Elements
{
    public abstract class Element // : IObjects
    {
        #region Const, properties

        public Flow Flow;
        public int Id { get; set; }

        private const double W = 50 * 1.5;
        private const double H = 25 * 1.5;
        private const double R = 5 * 1.5;
        private readonly SolidColorBrush _connectingEllipseBrush = Brushes.Black;
        private readonly SolidColorBrush _selectedConnectingEllipseBrush = Brushes.Red;

        protected Element()
        {
            Flow = new Flow(0, 0, 0);
            Id = Scheme.Count;
            Scheme.Count++;
        }

        #region width, height, radius, color, selected

        protected double Width
        {
            get => W;
        }

        protected double Height
        {
            get => H;
        }

        protected double Radius
        {
            get => R;
        }
        protected Brush elemBrush { get; set; }
        protected Color ElemColor { get; set; }

        protected SolidColorBrush GetConnectingEllipseBrush()
        {
            return _connectingEllipseBrush;
        }

        public SolidColorBrush GetSelectedConnectingEllipseBrush()
        {
            return _selectedConnectingEllipseBrush;
        }

        public bool Selected { get; set; }

        #endregion

        #region input, output

        public Vertex OutElement // элемент к которому идет выход (эл-т со входом и номер его входа)
        {
            get;
            set;
        }

        public List<Element> InputElements // элементы (выходы) которые соединены с входами данного эл-та
        {
            get;
            set;
        }

        public static void CheckElemCountInList(List<Element> list, int numb)
        {
            if (list.Count < numb + 1)
                for (int i = list.Count; i <= numb; i++)
                    list.Add(null);
        }

        public Point OutPoint // точка - вход эл-та
        {
            get;
            set;
        }

        public List<Point> InputPoints { get; set; }

        #endregion

        #region locationPoint

        private Point _locationPoint;

        public Point LocationPoint
        {
            get => _locationPoint;
            set => _locationPoint = value;
        }

        #endregion

        #endregion

        #region DataTables

        public abstract DataTable CreateDataTableResults();
        public abstract DataTable CreateDataTableProperties();
        public abstract void SetPropertiesFromDataTable(DataTable dt);

        protected DataTable CreateDataTable(string[] rowsParameter, double[] rowsValue)
        {
            int count;
            if (rowsParameter.Length != rowsValue.Length)
                count = Math.Min(rowsParameter.Length, rowsValue.Length);
            else
                count = rowsParameter.Length;

            DataTable dt = new DataTable();
            dt.Columns.Add("Параметр");
            dt.Columns.Add("Значение");

            for (int i = 0; i < count; i++)
            {
                DataRow dataTableRow = dt.NewRow();
                dataTableRow[0] = rowsParameter[i];
                dataTableRow[1] = rowsValue[i];
                dt.Rows.Add(dataTableRow);
            }

            return dt;
        }

        #endregion

        #region Draw

        public void Draw(Canvas PaintSurface)
        {
            Rectangle rect = DrawRectangle(PaintSurface);
            rect.Fill = this.elemBrush;
            //rect.Fill = new SolidColorBrush(ElemColor);
            DrawInOut(PaintSurface);

        }

        public void DrawChoosedElem(Canvas PaintSurface)
        {
            Rectangle rect = DrawRectangle(PaintSurface);
            rect.Stroke = Brushes.Green;
            rect.StrokeThickness = 2;

        }

        public void DrawConnections(Canvas PaintSurface)
        {
            if (this.OutElement != null & this.OutPoint != default(Point))
            {
                Lines.Draw(PaintSurface, this.OutPoint, this.OutElement.elem.InputPoints[this.OutElement.indexInList]);
            }
        }

        public abstract void DrawInOut(Canvas PaintSurface);

        protected Rectangle DrawRectangle(Canvas PaintSurface)
        {
            Rectangle rect = new Rectangle();
            rect.Width = this.Width;
            rect.Height = this.Height;
            PaintSurface.Children.Add(rect);
            Canvas.SetLeft(rect, this.LocationPoint.X);
            Canvas.SetTop(rect, this.LocationPoint.Y);
            return rect;
        }

        public Ellipse DrawEllipse(Canvas PaintSurface, Point location, SolidColorBrush brush)
        {
            Ellipse elps = new Ellipse();
            elps.Width = this.Radius;
            elps.Height = this.Radius;
            elps.Fill = brush;
            PaintSurface.Children.Add(elps);

            Canvas.SetLeft(elps, location.X);
            Canvas.SetTop(elps, location.Y);

            return elps;
        }

        #endregion

        #region CheckLocation

        protected bool CheckPointBelongElement(Point locationPoint, Point checkingPoint, double dxRight, double dxLeft,
            double dyTop, double dyBottom)
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


        public virtual bool CheckInput(Point checkingPoint, out Vertex vertex)
        {
            int deltaOutside = 10;
            int deltaInside = 2;
            if (this.InputPoints != null) // у эл-та предусмотрен вход (несколько)
            {
                foreach (Point inPoint in this.InputPoints)
                    if (CheckPointBelongElement(inPoint, checkingPoint, deltaInside * this.Radius, deltaOutside * this.Radius, deltaOutside * this.Radius,
                        deltaOutside * this.Radius))
                    {
                        vertex = new Vertex(this, this.InputPoints.IndexOf(inPoint));
                        return true;
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

        #region Drag

        public void Drag(double dX, double dY)
        {
            this._locationPoint.X = this._locationPoint.X + dX;
            this._locationPoint.Y = this._locationPoint.Y + dY;
        }

        #endregion

        #region Remove

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


        #endregion

        public abstract Flow Calculate();

        public virtual void SaveElement(StreamWriter sw)
        {
            sw.Write(this.Id + " ");
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.LocationPoint.X + " ");
            sw.Write(this.LocationPoint.Y + " ");
            if (this.OutElement!= null)
                sw.WriteLine(this.OutElement.elem.Id);
            else
                sw.WriteLine("");
        }

        public virtual void LoadElement(StreamReader sr)
        {
            //sw.Write(this.Id + " ");
            //sw.Write(this.GetType().Name + " ");
            //sw.Write(this.LocationPoint.X + " ");
            //sw.Write(this.LocationPoint.Y + " ");
            //if (this.OutElement != null)
            //    sw.WriteLine(this.OutElement.elem.Id);
            //else
            //    sw.WriteLine("");
        }

    }
}

//public interface IObjects
    //{
    //    void SetLocationPoints(List<Point> points);
    //    List<Point> GetLocationPoints();

    //    void Draw(Canvas PaintSurface);
    //    void DrawChoosedElem(Canvas PaintSurface);
    //    bool IsSelected();
    //    int CheckLocation(Point checkingPoint, int var);
    //    void Drag(int newX, int newY);

    //}

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

    //    public void Draw(Canvas PaintSurface)
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
    //        PaintSurface.Children.Add(line);

    //    }
    //    public void DrawChoosedElem(Canvas PaintSurface)
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


