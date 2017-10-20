using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using TestScheme.Schemes.Objects;
using TestScheme.Schemes.Objects.Elements;

namespace TestScheme.Schemes
{
    public class Scheme
    {
        public static List<Element> SchemeElements;
        //public static int Count;
        //public static Dictionary<int, int> Connections;

        #region Save, Load

        public static void Save()
        {
            string path = @"Saved\2.txt";

            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(SchemeElements.Count);
                foreach (Element elem in SchemeElements)
                    elem.SaveElement(sw);

                foreach (Element elem in SchemeElements)
                    elem.SaveElementConnections(sw);

            }

        }

        public static void Load()
        {
            string path = @"Saved\2.txt";
            string line = "";

            using (StreamReader sr = File.OpenText(path))
            {
                int.TryParse(sr.ReadLine(), out int count);
                SchemeElements = new List<Element>(count);

                for (int i = 0; i < count; i++)
                {
                    SchemeElements.Add(ElementFactory.Create(sr.ReadLine()));
                }

                while ((line = sr.ReadLine()) != null)
                {
                    string[] connections = line.Split(' ');
                    if (connections.Length == 3)
                    {
                        int.TryParse(connections[0], out int start);
                        int.TryParse(connections[1], out int end);
                        int.TryParse(connections[2], out int index);

                        SchemeElements[start].OutElement = new Vertex(SchemeElements[end], index);
                        Element.CheckElemCountInList(SchemeElements[end].InputElements, index);
                        SchemeElements[end].InputElements[index] = SchemeElements[start];
                    }



                }

            }
        }

        //private static void GetConnections(StreamWriter sw, out int count)
        //{
        //    count = 0;
        //    foreach (Element elem in SchemeElements)
        //        if (elem is Terminal)
        //            elem.SaveElementConnections(sw, ref count);
        //}
        #endregion
    }

}
