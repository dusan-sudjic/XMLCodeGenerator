using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements
{
    public sealed class CimRelationshipElement: Element, ICimProperty
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(IExpression) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return 1; } }
        public override string XML_Name { get { return "CimRelationship"; } }
        public CimRelationshipElement() : base()
        {
            Attributes.Add(new Attribute("name", true, ValueType.STRING));
            Attributes.Add(new Attribute("mandatory", false, ValueType.BOOLEAN));
        }
    }
}
