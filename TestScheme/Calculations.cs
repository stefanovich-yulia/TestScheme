using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestScheme.Schemes.Objects.Elements;


namespace TestScheme
{
    public static class Calculations
    {
        public const double Cwater = 4.2;//00; // теплоемкость Дж/ (кг*град)
        public const double Coil = 2.1;//00;
        public const double DensityWater = 1000; // плотность кг/м3
        public const double DensityOil = 800; // кг/м3

        public static Schemes.Flow CalculateSheme(List<Element> scheme)
        {
            foreach (Element elem in scheme)
            {
                if (elem is Terminal)
                {
                    return elem.Calculate();
                }
            }
            return null;
        }

    }
}
