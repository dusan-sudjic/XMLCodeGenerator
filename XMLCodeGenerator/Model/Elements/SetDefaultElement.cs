﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements
{
    public sealed class SetDefaultElement: Element, IExpression
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 0; } }
        public override int MaxContentSize { get { return 0; } }
        public override string XML_Name { get { return "SetDefault"; } }
        public SetDefaultElement() : base()
        {
            Attributes.Add(new Attribute("Value", true, ValueType.STRING));
        }
    }
}