using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestScheme.Schemes.Objects.Elements
{
    public class HeatExchanger : Element
    {
        // св-ва
        #region конструктор
        public HeatExchanger()
        {
            ElemColor = Color.FromRgb(255, 15, 48);
            elemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"heat2.jpg", UriKind.Relative)) };
            InputPoints = new List<Point>();
            InputElements = new List<Element>(2);
        }

        #endregion


        public override void DrawInOut(Canvas PaintSurface)
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

            DrawEllipse(PaintSurface, this.OutPoint, GetConnectingEllipseBrush());
            DrawEllipse(PaintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
            DrawEllipse(PaintSurface, this.InputPoints[1], GetConnectingEllipseBrush());
        }

        #region DataTable
        public override DataTable CreateDataTableProperties()
        {
            return null;
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
        public override void SetPropertiesFromDataTable(DataTable dt) { }
        #endregion

        public override Flow Calculate()
        {
            if (this.InputElements.Count == 0)
            {
                return new Flow(0, 0, 0);
            }
            else
            {
                double tmpTempreture = 0, tmpVoil = 0, tmpVwater = 0; 
                int countOfEmptyElem = 0;

                foreach (Element elem in this.InputElements)
                    if (elem.InputElements!= null && elem.InputElements.Count == 0)
                        countOfEmptyElem++;
                int n = this.InputElements.Count - countOfEmptyElem;

                foreach (Element elem in this.InputElements)
                {
                    tmpTempreture += elem.Calculate().Tempreture / n;
                    tmpVoil += elem.Calculate().Voil;
                    tmpVwater += elem.Calculate().Vwater;
                }

                this.Flow = new Flow(tmpVwater, tmpVoil, tmpTempreture);
                return this.Flow;
            }
        }

        public override bool CheckInput(Point checkingPoint, out Vertex vertex)
        {
            int deltaHorizontal = 3;
            int deltaVertical = 1;
            if (this.InputPoints != null)
            {
                foreach (Point inPoint in this.InputPoints)
                    if (CheckPointBelongElement(inPoint, checkingPoint, deltaHorizontal * this.Radius, deltaHorizontal * this.Radius,
                        deltaVertical * this.Radius, deltaVertical * this.Radius))
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
    }
}
