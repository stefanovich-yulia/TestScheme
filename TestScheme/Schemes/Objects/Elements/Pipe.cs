using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestScheme.Schemes.Objects.Elements
{
    public class Pipe : Element
    {
        // св-ва
        public double Asperity { get; set; }  // шероховатость
        public double Length { get; set; }
        public double Diameter { get; set; }
        #region конструктор
        public Pipe()
        {
            ElemColor = Color.FromRgb(109, 110, 96);
            elemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"pipe.jpg", UriKind.Relative)) };

            InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }
        #endregion

        public override void DrawInOut(Canvas PaintSurface)
        {
            double x = this.LocationPoint.X + this.Width - (double)Radius / 2;
            double y = this.LocationPoint.Y + this.Height / 2 - (double)Radius / 2;
            this.OutPoint = new Point(x, y);

            this.InputPoints.Clear();
            x = this.LocationPoint.X - (double)Radius / 2;
            this.InputPoints.Add(new Point(x, y));

            DrawEllipse(PaintSurface, this.OutPoint, GetConnectingEllipseBrush());
            DrawEllipse(PaintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
        }
        #region DataTable
        public override DataTable CreateDataTableProperties()
        {
            string[] rowsParameter = new string[3];
            double[] rowsValue = new double[3];
            rowsParameter[0] = "Шероховатость";
            rowsParameter[1] = "Длина, м";
            rowsParameter[2] = "Диаметр, м";
            rowsValue[0] = Asperity;
            rowsValue[1] = Length;
            rowsValue[2] = Diameter;

            return CreateDataTable(rowsParameter, rowsValue); 
        }
        public override DataTable CreateDataTableResults()
        {
            string[] rowsParameter = new string[3];
            double[] rowsValue = new double[3];
            rowsParameter[0] = "Qн, м3/сут";
            rowsParameter[1] = "Qв, м3/сут";
            rowsParameter[2] = "T, C";
            rowsValue[0] = Flow.Voil;
            rowsValue[1] = Flow.Vwater;
            rowsValue[2] = Flow.Tempreture;

            return CreateDataTable(rowsParameter, rowsValue); 

        }

        public override void SetPropertiesFromDataTable(DataTable dt)
        {
            double.TryParse(dt.Rows[0][1].ToString(), out double tmpAsperity);
            double.TryParse(dt.Rows[1][1].ToString(), out double tmpLength);
            double.TryParse(dt.Rows[2][1].ToString(), out double tmpDiameter);

            this.Flow = new Flow(tmpAsperity, tmpLength, tmpDiameter);
        }

        #endregion
        public override Flow Calculate()
        {
            if (this.InputElements.Count == 0)
                return new Flow(0,0,0);
            else
            {              
                Flow childFlow = this.InputElements[0].Calculate();
                double tmpVwater = childFlow.Vwater;
                double tmpVoil = childFlow.Voil;
                double tmpTempreture = childFlow.Tempreture;

                tmpVwater = tmpVwater * 0.75;
                tmpVoil = tmpVoil * 0.75;

                this.Flow = new Flow(tmpVwater, tmpVoil, tmpTempreture);

                return this.Flow;
            }
        }

        public override void SaveElement(StreamWriter sw)
        {
            sw.Write(this.Id + " ");
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.LocationPoint.X + " ");
            sw.Write(this.LocationPoint.Y + " ");
            sw.Write(this.Asperity + " ");
            sw.Write(this.Length + " ");
            sw.Write(this.Diameter + " ");
            if (this.OutElement != null)
                sw.WriteLine(this.OutElement.elem.Id);
            else
                sw.WriteLine("");
        }


    }
}
