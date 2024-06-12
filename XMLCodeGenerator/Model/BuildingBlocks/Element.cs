using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public abstract class Element : IElement
    {
        public List<IElement> ChildElements { get; set; }
        public int ContentSize { get; } //-1 = inf
        public abstract List<Type> SupportedChildElementTypes { get; }
        public HashSet<Attribute> Attributes{ get; set; }
        public abstract int MinContentSize { get; }
        public abstract int MaxContentSize { get; }
        public abstract string XML_Name { get; }
        public Element()
        {
            Attributes = new();
            ChildElements = new();
        }
        public virtual bool AddChildElementToContent(IElement newElement)
        {
            if (ChildElements.Count == MaxContentSize) return false;
            if (SupportedChildElementTypes.Any(t => t.IsAssignableFrom(newElement.GetType())))
            {
                ChildElements.Add(newElement);
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return XML_Name;
        }
    }

}
