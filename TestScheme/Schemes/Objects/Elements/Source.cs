using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TestScheme.DrawingElements;

namespace TestScheme.Schemes.Objects.Elements
{
    public class Source : Element
    {
        // св-ва

        #region конструктор
        public Source()
        {
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"source.png", UriKind.Relative)) };
            Flow = new Flow(100,500,59, 50);
        }

        public Source(string[] parameters) : base(parameters)
        {
            Flow = new Flow(100, 500, 59, 50); 

            double.TryParse(parameters[4], out double tmp);
            this.Flow.Goil = tmp;
            double.TryParse(parameters[5], out tmp);
            this.Flow.Gwater = tmp;
            double.TryParse(parameters[6], out tmp);
            this.Flow.Tempreture = tmp;

            //ElemColor = Color.FromRgb(7, 199, 255);
            //ElemBrush = new SolidColorBrush(ElemColor);
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"source.png", UriKind.Relative)) };

        }

        protected override void SetInputOutputPoints()
        {
            double x = this.LocationPoint.X + Shapes.Width - Shapes.Radius / 2;
            double y = this.LocationPoint.Y + Shapes.Height / 2 - Shapes.Radius / 2;
            this.OutPoint = new Point(x, y);
        }
        #endregion

        #region Draw, Calculate

        public override Flow Calculate()
        {
            return this.Flow;
        }
        #endregion

        #region DataTables
        public override DataTable CreateDataTableProperties()
        {
            int count = 4;
            string[] rowsParameter = new string[count];
            double[] rowsValue = new double[count];

            rowsParameter[0] = "Gн, м3/сут";
            rowsParameter[1] = "Gв, м3/сут";
            rowsParameter[2] = "T, C";
            rowsParameter[3] = "p, атм";

            rowsValue[0] = Flow.Goil;
            rowsValue[1] = Flow.Gwater;
            rowsValue[2] = Flow.Tempreture;
            rowsValue[3] = Flow.Pressure;

            return CreateDataTable(rowsParameter, rowsValue);
        }

        //public override DataTable CreateDataTableResults()
        //{
        //    return null;
        //}
        public override void ChangePropertyByUser(double property, int row, int column)
        {
            if (column == 1)
            {
                switch (row)
                {
                    case 0:
                        this.Flow.Goil = property;
                        break;
                    case 1:
                        this.Flow.Gwater = property;
                        break;
                    case 2:
                        this.Flow.Tempreture = property;
                        break;

                }
            }
        }
        //public override void SetPropertiesFromDataTable(DataTable dt)
        //{
        //    double.TryParse(dt.Rows[0][1].ToString(), out double tmpGoil);
        //    double.TryParse(dt.Rows[1][1].ToString(), out double tmpGwater);
        //    double.TryParse(dt.Rows[2][1].ToString(), out double tmpTempreture);

        //    this.Flow = new Flow(tmpGwater, tmpGoil, tmpTempreture);
        //}
        #endregion

        public override void SaveElement(StreamWriter sw)
        {
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.Id + " ");
            
            sw.Write(this.LocationPoint.X + " ");
            sw.Write(this.LocationPoint.Y + " ");
            sw.Write(this.Flow.Goil + " ");
            sw.Write(this.Flow.Gwater + " ");
            sw.WriteLine(this.Flow.Tempreture);
            //if (this.OutElement != null)
            //    sw.WriteLine(this.OutElement.elem.Id);
            //else
            //    sw.WriteLine("");
        }

    }
}
