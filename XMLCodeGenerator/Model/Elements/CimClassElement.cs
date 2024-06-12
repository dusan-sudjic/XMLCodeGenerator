using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements
{
    public sealed class CimClassElement : Element, ICim
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(ICimProperty) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return -1; } }
        public override string XML_Name { get { return "CimClass"; } }

        public CimClassElement() : base() 
        {
            Attributes.Add(new Attribute("name", true, ValueType.STRING));
            Attributes.Add(new Attribute("source", true, ValueType.STRING));
        }
    }
}
