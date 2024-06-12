using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements
{
    public class FunctionCallElement : Element, IElement
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(ICimProperty), typeof(IExpression), typeof(IBooleanOperator), typeof(ICondition) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return -1; } }
        public override string XML_Name { get { return "Function"; } }
        public FunctionCallElement() : base() 
        {
            Attributes.Add(new Attribute("name", true, ValueType.STRING));
        }
        public static FunctionCallElement CreateNewInstance()
        {
            return new FunctionCallElement();
        }
    }
}
