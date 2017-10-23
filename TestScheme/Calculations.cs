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
        public const double Cwater = 4200;//00; // теплоемкость Дж/ (кг*град)
        public const double Coil = 2100;//00;
        public const double DensityWater = 1000; // плотность кг/м3
        public const double DensityOil = 800; // 
        public const double ViscosityWater = 0.000000294; // вязкость м2/c при 100град
        public const double ViscosityOil = 0.000000294; //


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
