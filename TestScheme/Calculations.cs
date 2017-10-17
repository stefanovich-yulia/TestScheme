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
