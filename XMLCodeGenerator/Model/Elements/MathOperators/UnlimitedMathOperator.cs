using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Elements.MathOperators
{
    public class UnlimitedMathOperator: NumberExpression
    {
        private string _XML_Name;
        public override int MinContentSize { get { return 2; } }
        public override int MaxContentSize { get { return -1; } }
        public override string XML_Name { get { return _XML_Name; } }
        public UnlimitedMathOperator(string name) : base()
        {
            _XML_Name = name;
        }
    }
    public sealed class AdditionElement: UnlimitedMathOperator { public AdditionElement(): base("Addition") { } }
    public sealed class MultiplicationElement: UnlimitedMathOperator { public MultiplicationElement(): base("Multiplication") { } }
}
