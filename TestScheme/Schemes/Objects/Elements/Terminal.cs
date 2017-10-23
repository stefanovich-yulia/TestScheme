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
    public class Terminal : Element
    {
        // св-ва
        #region конструктор
        public Terminal()
        {
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"terminal.png", UriKind.Relative)) };

            //InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }
        public Terminal(System.String[] parameters) : base(parameters)
        {
            //ElemColor = Color.FromRgb(227, 228, 209);
            //ElemBrush = new SolidColorBrush(ElemColor);
            ElemBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"terminal.png", UriKind.Relative)) };

            //InputPoints = new List<Point>();
            InputElements = new List<Element>(1);
        }

        #endregion

        #region Draw
        protected override void SetInputOutputPoints()
        {
            this.InputPoints = new List<Point>();
            double x = LocationPoint.X - Shapes.Radius / 2;
            double y = LocationPoint.Y + Shapes.Height / 2 - Shapes.Radius / 2;
            this.InputPoints.Add(new Point(x, y));
        }


        #endregion

        #region DataTable

        public override void ChangePropertyByUser(double property, int row, int column) { }
        public override DataTable CreateDataTableProperties()
        {
            return null;
        }
        //public override void SetPropertiesFromDataTable(DataTable dt) { }
        #endregion

        #region Calculate
        public override Flow Calculate()
        {
            if (this.InputElements.Count == 0)
                return new Flow();
            else
            {
                this.Flow = this.InputElements[0].Calculate();
                return this.Flow;
            }
        }
        #endregion

    }
}
