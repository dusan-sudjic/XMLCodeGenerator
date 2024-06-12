using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.MathOperators
{
    public class UnaryMathOperation: NumberExpression
    {
        private string _XML_Name;
        public override int MinContentSize { get { return 1; } }
        public override int MaxContentSize { get { return 1; } }
        public override string XML_Name { get { return _XML_Name; } }
        public UnaryMathOperation(string name) : base()
        {
            _XML_Name = name;
        }
        public class AbsoluteValueElement: UnaryMathOperation { public AbsoluteValueElement(): base("AbsoluteValue") { } }
        public class MathSqrtElement: UnaryMathOperation { public MathSqrtElement(): base("MathSqrt") { } }
        public class MathCeilingElement : UnaryMathOperation { public MathCeilingElement(): base("MathCeiling") { } }
        public class MathFloorElement: UnaryMathOperation { public MathFloorElement(): base("MathFloor") { } }
        public class MathRoundElement : UnaryMathOperation { public MathRoundElement(): base("MathRound") { } }
        public class MathTruncateElement: UnaryMathOperation { public MathTruncateElement(): base("MathTruncate") { } }
        public class MatchCosElement: UnaryMathOperation { public MatchCosElement(): base("MatchCos") { } }
        public class MathSinElement : UnaryMathOperation { public MathSinElement(): base("MathSin") { } }
        public class MathTanElement: UnaryMathOperation { public MathTanElement(): base("MathTan") { } }
    }
}
