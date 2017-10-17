using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestScheme.Schemes.Objects.Elements
{
    public class Terminal : Element
    {
        // св-ва
        #region конструктор
        public Terminal()
        {
            ElemColor = Color.FromRgb(227, 228, 209);
            elemBrush = new SolidColorBrush(ElemColor);

            InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }

        #endregion

        public override void DrawInOut(Canvas PaintSurface)
        {
            this.InputPoints.Clear();
            double x = LocationPoint.X - Radius / 2;
            double y = LocationPoint.Y + this.Height / 2 - Radius / 2;
            this.InputPoints.Add(new Point(x, y));

            DrawEllipse(PaintSurface, this.InputPoints[0], GetConnectingEllipseBrush());
        }
        public override DataTable CreateDataTableProperties()
        {
            return null;
        }
        
        #region DataTable
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
                return new Flow(0,0,0);
            else
            {
                this.Flow = this.InputElements[0].Calculate();
                return this.Flow;
            }
        }

    }
}
