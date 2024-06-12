using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.BooleanOperators
{
    public class NumericComparator : Element, IBooleanOperator
    {
        private List<Type> _SupportedChildElementTypes = new List<Type> { typeof(INumberExpression) };
        public override List<Type> SupportedChildElementTypes { get { return _SupportedChildElementTypes; } }
        private string _XML_Name;
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return 2; } }
        public override string XML_Name { get { return _XML_Name; } }
        public NumericComparator(string name) : base()
        {
            _XML_Name = name;
        }
    }
    public sealed class GreaterThanElement: NumericComparator { public GreaterThanElement() : base("GreaterThan") { } }
    public sealed class GreaterThanOrEqualElement: NumericComparator { public GreaterThanOrEqualElement() : base("GreaterThanOrEqual") { } }
    public sealed class LessThanElement: NumericComparator { public LessThanElement() : base("LessThan") { } }
    public sealed class LessThanOrEqualElement : NumericComparator { public LessThanOrEqualElement() : base("LessThanOrEqual") { } }
}
