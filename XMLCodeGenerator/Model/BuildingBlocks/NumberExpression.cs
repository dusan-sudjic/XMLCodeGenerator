using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public abstract class NumberExpression : Element, INumberExpression
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(INumberExpression) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        public NumberExpression() : base() { }
    }
}
