using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestScheme.Schemes.Objects.Elements
{
    class ElementFactory
    {
        private static readonly Dictionary<string, Type> ElementsTypesDictionary =
            new Dictionary<string, Type>
            {
                {"Source", typeof(Source)},
                {"Pipe", typeof(Pipe)},
                {"HeatExchanger", typeof(HeatExchanger)},
                {"Terminal", typeof(Terminal)}
            };


        public static Element Create(string line)
        {
            string[] parameters = line.Split(' ');
            Element element = null;

            if (ElementsTypesDictionary.TryGetValue(parameters[0],  out Type elemType))
            {
                ConstructorInfo constructor = elemType.GetConstructor(new Type[] { typeof(string[]) });
                if (constructor != null)
                    element = (Element)constructor.Invoke(new object[1] {parameters});
            }
            return element;
        }
    }
}
