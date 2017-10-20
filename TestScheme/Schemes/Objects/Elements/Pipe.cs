using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TestScheme.DrawingElements;

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
            //ElemColor = Color.FromRgb(109, 110, 96);
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"pipe.jpg", UriKind.Relative)) };
            InputElements = new List<Element>(1);
        }
        public Pipe(System.String[] parameters) : base(parameters)
        {
            double.TryParse(parameters[4], out double tmp);
            this.Asperity = tmp;
            double.TryParse(parameters[5], out tmp);
            this.Length = tmp;
            double.TryParse(parameters[6], out tmp);
            this.Diameter = tmp;

            //ElemColor = Color.FromRgb(109, 110, 96);
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"pipe.jpg", UriKind.Relative)) };

            //InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }
        #endregion

        #region Draw

        protected override void SetInputOutputPoints()
        {
            double x = this.LocationPoint.X + Shapes.Width - (double)Shapes.Radius / 2;
            double y = this.LocationPoint.Y + Shapes.Height / 2 - (double)Shapes.Radius / 2;
            this.OutPoint = new Point(x, y);

            this.InputPoints = new List<Point>();
            x = this.LocationPoint.X - (double)Shapes.Radius / 2;
            this.InputPoints.Add(new Point(x, y));
        }
        #endregion

        #region DataTable
        public override DataTable CreateDataTableProperties()
        {
            string[] rowsParameter = new string[3];
            double[] rowsValue = new double[3];
            rowsParameter[0] = "Шероховатость, мкм"; 
            rowsParameter[1] = "Длина, м";
            rowsParameter[2] = "Диаметр, м";
            rowsValue[0] = Asperity;
            rowsValue[1] = Length;
            rowsValue[2] = Diameter;

            return CreateDataTable(rowsParameter, rowsValue); 
        }
        //public override DataTable CreateDataTableResults()
        //{

        //}

        public override void ChangePropertyByUser(double property, int row, int column)
        {
            if (column == 1)
            {
                switch (row)
                {
                    case 0:
                        this.Asperity = property;
                        break;
                   case 1:
                        this.Length = property;
                        break;
                    case 2:
                        this.Diameter = property;
                        break;

                }
            }
        }
        //public override void SetPropertiesFromDataTable(DataTable dt)
        //{
        //    double.TryParse(dt.Rows[0][1].ToString(), out double tmpAsperity);
        //    double.TryParse(dt.Rows[1][1].ToString(), out double tmpLength);
        //    double.TryParse(dt.Rows[2][1].ToString(), out double tmpDiameter);

        //    this.Flow = new Flow(tmpAsperity, tmpLength, tmpDiameter);
        //}

        #endregion

        #region Calculate
        public override Flow Calculate()
        {
            if (this.InputElements.Count == 0)
                return new Flow();
            else
            {              
                Flow childFlow = this.InputElements[0].Calculate();
                double tmpGwater = childFlow.Gwater;
                double tmpGoil = childFlow.Goil;
                double tmpTempreture = childFlow.Tempreture;
                double tmpPressure = childFlow.Pressure;

                tmpGwater = tmpGwater * 0.75;
                tmpGoil = tmpGoil * 0.75;

                this.Flow = new Flow(tmpGwater, tmpGoil, tmpTempreture, tmpPressure);

                return this.Flow;
            }
        }
        #endregion

        public override void SaveElement(StreamWriter sw)
        {
            sw.Write(this.GetType().Name + " ");
            sw.Write(this.Id + " ");
           
            sw.Write(this.LocationPoint.X + " ");
            sw.Write(this.LocationPoint.Y + " ");
            sw.Write(this.Asperity + " ");
            sw.Write(this.Length + " ");
            sw.WriteLine(this.Diameter);
            //if (this.OutElement != null)
            //    sw.WriteLine(this.OutElement.elem.Id);
            //else
            //    sw.WriteLine("");
        }


    }
}
