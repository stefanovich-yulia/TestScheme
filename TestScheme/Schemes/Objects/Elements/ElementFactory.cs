using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            object element = null;

            if (ElementsTypesDictionary.TryGetValue(parameters[0],  out Type elemType))
            {
                ConstructorInfo constructor = elemType.GetConstructor(new Type[1] { typeof(string[]) });
                if (constructor != null)
                    element = constructor.Invoke(new object[1] {parameters});
            }
            return (Element)element;
    }

    }
}
