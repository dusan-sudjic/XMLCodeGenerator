using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.Conditions
{
    public sealed class IfElement : Element, IExpression
    {
        //public ICondition Condition { get; set; }
        //public IExpression Expression { get; set; }
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(IExpression), typeof(ICondition) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return 2; } }
        public override string XML_Name { get { return "If"; } }
        public IfElement() : base() { }
        public override bool AddChildElementToContent(IElement newElement)
        {
            if (ChildElements.Count == MaxContentSize) return false;
            if (ChildElements.Count == 0)
            {
                if (!(newElement is ICondition))
                    return false;
                ChildElements.Add((ICondition)newElement);
                return true;
            }
            else
            {
                if (!(newElement is IExpression))
                    return false;
                ChildElements.Add((IExpression)newElement);
                return true;
            }
        }
    }
}
