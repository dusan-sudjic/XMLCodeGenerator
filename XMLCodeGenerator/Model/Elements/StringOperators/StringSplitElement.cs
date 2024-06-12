using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.StringOperators
{
    public sealed class StringSplitElement: Expression
    {
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return 1; } }
        public override string XML_Name { get { return "StringSplit"; } }
        public StringSplitElement(): base()
        {
            Attributes.Add(new Attribute("Separator", true, ValueType.STRING));
            Attributes.Add(new Attribute("Index", true, ValueType.INTEGER));
        }
    }
}
