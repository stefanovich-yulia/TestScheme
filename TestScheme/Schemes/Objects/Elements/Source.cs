using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestScheme.Schemes.Objects.Elements
{
    public class Source : Element
    {
        // св-ва

        #region конструктор
        public Source()
        {
            ElemColor = Color.FromRgb(7, 199, 255);
            elemBrush = new SolidColorBrush(ElemColor);
            Flow = new Flow(25,5,59);
        }

        #endregion
        public override void DrawInOut(Canvas PaintSurface)
        {
            double x = this.LocationPoint.X + this.Width - this.Radius / 2;
            double y = this.LocationPoint.Y + this.Height / 2 - this.Radius / 2;
            this.OutPoint = new Point(x, y);

            DrawEllipse(PaintSurface, this.OutPoint, GetConnectingEllipseBrush());
        }

        public override Flow Calculate()
        {
            return this.Flow;
        }

        #region DataTables
        public override DataTable CreateDataTableProperties()
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

        public override DataTable CreateDataTableResults()
        {
            return null;
        }

        public override void SetPropertiesFromDataTable(DataTable dt)
        {
            double.TryParse(dt.Rows[0][1].ToString(), out double tmpVoil);
            double.TryParse(dt.Rows[1][1].ToString(), out double tmpVwater);
            double.TryParse(dt.Rows[2][1].ToString(), out double tmpTempreture);

            this.Flow = new Flow(tmpVwater, tmpVoil, tmpTempreture);
        }
        #endregion

        public override void SaveElement(StreamWriter sw)
        {
            sw.Write(this.Id + " ");
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.LocationPoint.X + " ");
            sw.Write(this.LocationPoint.Y + " ");
            sw.Write(this.Flow.Voil + " ");
            sw.Write(this.Flow.Vwater + " ");
            sw.Write(this.Flow.Tempreture + " ");
            if (this.OutElement != null)
                sw.WriteLine(this.OutElement.elem.Id);
            else
                sw.WriteLine("");
        }

    }
}
