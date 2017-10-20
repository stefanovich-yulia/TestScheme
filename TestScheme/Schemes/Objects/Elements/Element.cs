using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

using TestScheme.DrawingElements;

namespace TestScheme.Schemes.Objects.Elements
{
    public abstract class Element // : IObjects
    {
        #region Const, properties

        public Flow Flow { get; set; }
        public int Id { get; set; }

        public static readonly SolidColorBrush SelectedConnectingEllipseBrush = Brushes.Red;
        public static readonly Brush ChoosedElementBrush = Brushes.Green;
        protected Brush ElemBrush { get; set; }

        //private const double W = 50 * 1.5;
        //private const double H = 25 * 1.5;
        //private const double R = 5 * 1.5;
        //private readonly SolidColorBrush _connectingEllipseBrush = Brushes.Black;

        //protected Color ElemColor { get; set; }
        //public bool Selected { get; set; }


        #region input, output

        public Vertex OutElement // элемент к которому идет выход данного эл-та (эл-т со входом и номер его входа)
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

        protected abstract void SetInputOutputPoints();

        #endregion

        #region locationPoint

        private Point _locationPoint;

        public Point LocationPoint
        {
            get => _locationPoint;
            set
            {
                _locationPoint = value; 
                SetInputOutputPoints();
            }
        }

        #endregion

        #endregion

        #region Constructors
        protected Element()
        {
            Flow = new Flow(0, 0, 0,0);
            Id = Scheme.SchemeElements.Count;
            //Scheme.Count++;
        }
        protected Element(string[] parameters)
        {
            Flow = new Flow(0, 0, 0,0);

            int.TryParse(parameters[1], out int id);
            this.Id = id;

            double.TryParse(parameters[2], out double x);
            double.TryParse(parameters[3], out double y);
            this.LocationPoint = new Point(x, y);
        }
        #endregion

        #region DataTables

        public DataTable CreateDataTableResults()
        {
            int count = 4;
            string[] rowsParameter = new string[count];
            double[] rowsValue = new double[count];

            rowsParameter[0] = "Gн, м3/сут";
            rowsParameter[1] = "Gв, м3/сут";
            rowsParameter[2] = "T, C";
            rowsParameter[3] = "p, атм.";

            rowsValue[0] = Flow.Goil;
            rowsValue[1] = Flow.Gwater;
            rowsValue[2] = Flow.Tempreture;
            rowsValue[3] = Flow.Pressure;

            return CreateDataTable(rowsParameter, rowsValue);
        }
        public abstract DataTable CreateDataTableProperties();
        //public abstract void SetPropertiesFromDataTable(DataTable dt);
        public abstract void ChangePropertyByUser(double property, int row, int column);

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

        public void Draw(Canvas paintSurface)
        {
            Rectangles rect = new Rectangles(this.ElemBrush, LocationPoint);
            rect.Draw(paintSurface);
            DrawInOut(paintSurface);
        }

        public void DrawChoosedElem(Canvas paintSurface)
        {
            Rectangles rect = new Rectangles(ChoosedElementBrush, LocationPoint);
            rect.Draw(paintSurface);
        }

        public void DrawConnections(Canvas paintSurface)
        {
            if (this.OutElement != null && this.OutPoint != default(Point) && this.OutElement.elem.InputPoints.Count != 0)
            {
                Lines line = new Lines(this.OutPoint, this.OutElement.elem.InputPoints[this.OutElement.indexInList]);
                line.Draw(paintSurface);
            }
        }

        public void DrawInOut(Canvas paintSurface)
        {
            if (this.OutPoint != default(Point))
            {
                Ellipses elps = new Ellipses(this.OutPoint);
                elps.Draw(paintSurface);
            }
            if (this.InputPoints != null)
            {
                Ellipses elps = new Ellipses();
                foreach (Point pnt in this.InputPoints)
                {
                    elps.LocationPoint = pnt;
                    elps.Draw(paintSurface);
                }
            }
        }

        #endregion

        #region CheckLocation

        protected static bool CheckPointBelongElement(Point locationPoint, Point checkingPoint, double dxLeft, double dxRight,
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
            double r = Shapes.Radius * 2;
            return CheckPointBelongElement(this.LocationPoint, checkingPoint, - r, Shapes.Width - r, 0, Shapes.Height);
        }

        public bool CheckOutput(Point checkingPoint)
        {
            double r = Shapes.Radius * 2;
            double rOutside = Shapes.Radius * 3.5;

            if (this.OutPoint != default(Point)) // у эл-та предусмотрен выход
            {
                if (CheckPointBelongElement(this.OutPoint, checkingPoint, r, rOutside, rOutside, rOutside))
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
            double r = Shapes.Radius * 2;
            double rOutside = Shapes.Radius * 3.5;
            if (this.InputPoints != null) // у эл-та предусмотрен вход (несколько)
            {
                foreach (Point inPoint in this.InputPoints)
                    if (CheckPointBelongElement(inPoint, checkingPoint, rOutside, r, rOutside, rOutside))
                    {
                        int index = this.InputPoints.IndexOf(inPoint);
                        if (this.InputElements.Count > index && this.InputElements[index] != null)
                        {
                            vertex = null;
                            return false;
                        }
                        else
                        {
                            vertex = new Vertex(this, this.InputPoints.IndexOf(inPoint));
                            return true;
                        }
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

        public static bool CheckLocationInCanvas(Canvas paintSurface, Point checkingPoint)
        {
            return CheckPointBelongElement(new Point(0, 0), checkingPoint, 0, paintSurface.ActualWidth, 0, paintSurface.ActualHeight);
        }


        #endregion

        #region Drag

        public void Drag(double dX, double dY)
        {
            double x = this.LocationPoint.X;
            double y = this.LocationPoint.Y;

            LocationPoint = new Point(x + dX, y + dY);
        }

        #endregion

        #region Remove

        public void RemoveElem()
        {
            int indexInScheme = this.Id;
            if (this.OutElement != null)
            {
                int indexInInputsList = this.OutElement.indexInList;
                if (Scheme.SchemeElements[Id].OutElement.elem.InputElements.Count >=
                    indexInInputsList)
                    Scheme.SchemeElements[Id].OutElement.elem.InputElements[indexInInputsList] =  null;
            }
            if (this.InputElements != null)
            {
                foreach (Element elem in this.InputElements)
                    elem.OutElement = null;
            }
            //Scheme.SchemeElements.Find(elem => elem == this).OutElement = null;
            Scheme.SchemeElements.Remove(this);

            for (int i = indexInScheme; i < Scheme.SchemeElements.Count; i++)
                Scheme.SchemeElements[i].Id = i;
        }


        #endregion

        #region Calculate
        public abstract Flow Calculate();
        #endregion

        #region Save
        public void SaveElementConnections(StreamWriter sw)
        {
            if (this.OutElement != null)
            {
                sw.WriteLine(this.Id + " " + this.OutElement.elem.Id + " " + this.OutElement.indexInList);
            }
        }
        public virtual void SaveElement(StreamWriter sw)
        {
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.Id + " ");
            
            sw.Write(this.LocationPoint.X + " ");
            sw.WriteLine(this.LocationPoint.Y);
            //if (this.OutElement!= null)
            //    sw.WriteLine(this.OutElement.elem.Id);
            //else
            //    sw.WriteLine("");
        }
        #endregion

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


