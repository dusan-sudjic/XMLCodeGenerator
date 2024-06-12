using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.StringOperators
{
    public sealed class StringConcatenateElement: Expression
    {
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return -1; } }
        public override string XML_Name { get { return "StringConcatenate"; } }
        public StringConcatenateElement(): base() 
        {
            Attributes.Add(new Attribute("delimiter", false, ValueType.STRING));
        }
    }
}
