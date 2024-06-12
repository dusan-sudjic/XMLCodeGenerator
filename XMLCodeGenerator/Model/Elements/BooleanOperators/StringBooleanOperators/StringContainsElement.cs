﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.BooleanOperators.StringBooleanOperators
{
    public sealed class StringContainsElement : BooleanOperator
    {
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return 1; } }
        public override string XML_Name { get { return "StringContains"; } }
        public StringContainsElement() : base()
        {
            Attributes.Add(new Attribute("Value", true, ValueType.STRING));
        }
    }
}
