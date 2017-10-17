using System;
using TestScheme.Schemes.Objects.Elements;

namespace TestScheme.Schemes.Objects
{
    public class Vertex
    {
        public Element elem { get; set; }
        public int indexInList { get; set; }

        public Vertex(Element elem, int indexInList)
        {
            this.elem = elem;
            this.indexInList = indexInList;
        }

    }
}
