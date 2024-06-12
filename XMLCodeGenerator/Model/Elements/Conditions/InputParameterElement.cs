using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.Conditions
{
    public sealed class InputParameterElement: Element, IInputParameter
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(IExpression) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return 1; } }
        public override string XML_Name { get { return "InputParameter"; } }
        public InputParameterElement() : base() { }
    }
}
