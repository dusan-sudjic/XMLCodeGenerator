using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.MathOperators
{
    public class BinaryMathOperation : NumberExpression
    {
        private string _XML_Name;
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return 2; } }
        public override string XML_Name { get { return _XML_Name; } }
        public BinaryMathOperation(string name) : base() 
        { 
            _XML_Name = name;
        }
    }
    public sealed class SubstractionElement : BinaryMathOperation { public SubstractionElement() : base("Substraction") { } }
    public sealed class DivisionElement : BinaryMathOperation { public DivisionElement() : base("Division") { } }
    public sealed class MathPowElement : BinaryMathOperation { public MathPowElement() : base("MathPow") { } }
    public sealed class MathMaxElement : BinaryMathOperation { public MathMaxElement() : base("MathMax") { } }
    public sealed class MathMinElement : BinaryMathOperation { public MathMinElement() : base("MathMin") { } }
}
