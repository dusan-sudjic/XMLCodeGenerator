using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.BooleanOperators
{
    public sealed class ContainsPropertyElement: BooleanOperator
    {
        public override int MinContentSize { get { return 0; } }
        public override int MaxContentSize { get { return 0; } }
        public override string XML_Name { get { return "ContainsProperty"; } }
        public ContainsPropertyElement(): base()
        {
            Attributes.Add(new Attribute("PropertyName", true, ValueType.STRING));
        }
    }
}
