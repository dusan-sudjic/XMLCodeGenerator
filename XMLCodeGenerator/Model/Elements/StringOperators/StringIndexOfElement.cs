using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.StringOperators
{
    public sealed class StringIndexOfElement: Expression
    {
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return 2; } }
        public override string XML_Name { get { return "StringIndexOf"; } }
        public StringIndexOfElement(): base() { }
    }
}
