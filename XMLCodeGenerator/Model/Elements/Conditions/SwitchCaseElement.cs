using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.Conditions
{
    public sealed class SwitchCaseElement: Element, IExpression
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(ICase), typeof(IInputParameter), typeof(IDefault) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public override int MinContentSize { get { return 3; } }
        public override int MaxContentSize { get { return -1; } }
        public override string XML_Name { get { return "SwitchCase"; } }
        public SwitchCaseElement() : base() { }
        public override bool AddChildElementToContent(IElement newElement)
        {
            if (ChildElements.Count == 0)
            {
                if (!(newElement is IInputParameter))
                    return false;
                ChildElements.Add((IInputParameter)newElement);
                return true;
            }
            else
            {
                if (newElement is ICase)
                {
                    ChildElements.Add((ICase)newElement);
                    return true;
                }
                if (newElement is IDefault)
                {
                    ChildElements.Add((IDefault)newElement);
                    return true;
                }
                return false;
            }
        }
    }
}
