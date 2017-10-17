using System;
using System.Collections.Generic;
using System.IO;
using TestScheme.Schemes.Objects.Elements;

namespace TestScheme.Schemes
{
    public class Scheme
    {
        public static List<Element> SchemeElements;
        public static int Count;
        //public static Dictionary<int, int> Connections;

        #region Save, Load

        public static void Save()
        {
            string path = @"Saved\2.txt";
            //Dictionary<int, int> d;

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(Count);
                foreach (Element elem in SchemeElements)
                    elem.SaveElement(sw);
            }

        }

        public static void Load()
        {
            string path = @"Saved\2.txt";

            using (StreamReader sr = File.OpenText(path))
            {
                
            }
        }
        #endregion
    }

}
