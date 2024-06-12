using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public abstract class Expression: Element, IExpression
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(IExpression) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public Expression() : base(){ }
    }
}
