using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.GetOperators
{
    public sealed class GetSourceTableElement: Element, IExpression
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 0; } }
        public override int MaxContentSize { get { return 0; } }
        public override string XML_Name { get { return "GetSourceTable"; } }
        public GetSourceTableElement() : base()
        {
            Attributes.Add(new Attribute("EntityName", true, ValueType.STRING));
        }
    }
}
