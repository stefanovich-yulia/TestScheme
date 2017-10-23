using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestScheme.DrawingElements;

namespace TestScheme.Schemes.Objects.Elements
{
    public class HeatExchanger : Element
    {

        #region конструктор
        public HeatExchanger()
        {
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"heat2.jpg", UriKind.Relative)) };
            InputElements = new List<Element>(2);
        }

        public HeatExchanger(System.String[] parameters) : base(parameters)
        {
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"heat2.jpg", UriKind.Relative)) };
            InputElements = new List<Element>(2);
        }

        #endregion

        #region CheckInput
        protected override void SetInputOutputPoints()
        {
            double x = LocationPoint.X + Shapes.Width - Shapes.Radius/2;
            double y = LocationPoint.Y + Shapes.Height / 2 - Shapes.Radius/2;
            this.OutPoint = new Point(x, y);

            this.InputPoints = new List<Point>();
            x = LocationPoint.X - Shapes.Radius / 2;
            y = LocationPoint.Y + Shapes.Radius;
            this.InputPoints.Add(new Point(x, y));
            y = LocationPoint.Y + Shapes.Height - 2 * Shapes.Radius;
            this.InputPoints.Add(new Point(x, y));
        }

        public override bool CheckInput(Point checkingPoint, out Vertex vertex)
        {
            double r = Shapes.Radius * 2;
            double rOutside = Shapes.Radius * 3.5;
            double rVertical = Shapes.Radius * 1.5;
            if (this.InputPoints != null)
            {
                foreach (Point inPoint in this.InputPoints)
                    if (CheckPointBelongElement(inPoint, checkingPoint, rOutside, r, rVertical, rVertical))
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
#endregion

        #region DataTable
        public override DataTable CreateDataTableProperties()
        {
            return null;
        }

        public override void ChangePropertyByUser(double property, int row, int column) { }
        #endregion

        #region Calculate
        public override Flow Calculate()
        {
            if (this.InputElements.Count == 0)
            {
                return new Flow();
            }
            else
            {
                double tempreture = 0, Goil = 0, Gwater = 0, pressure = double.MaxValue;

                foreach (Element elem in this.InputElements)
                {
                    Flow tmpFlow = elem.Calculate();

                    Goil += tmpFlow.Goil;
                    Gwater += tmpFlow.Gwater;
                    tempreture += tmpFlow.Tempreture * 
                        (tmpFlow.Goil * Calculations.Coil * Calculations.DensityOil + 
                        tmpFlow.Gwater * Calculations.Cwater * Calculations.DensityWater);

                    pressure = Math.Min(pressure, tmpFlow.Pressure);
                }
                
                tempreture = Math.Round(tempreture / 
                    (Goil* Calculations.Coil * Calculations.DensityOil + Gwater * Calculations.Cwater * Calculations.DensityWater));

                this.Flow = new Flow(Gwater, Goil, tempreture, pressure);
                return this.Flow;
            }
        }
        #endregion
  
    }
}
