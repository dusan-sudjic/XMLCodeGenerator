using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model.BuildingBlocks.Implementations
{

    public class Expression : Element, IExpression
    {
        public Expression(ElementBlueprint blueprint) : base(blueprint) { }
        public override string ToXML(int depth)
        {
            if (!XML_Name.Equals("IfBlock"))
                return base.ToXML(depth);
            else
            {
                string xml = "";
                foreach (var el in ChildElements)
                    xml += el.ToXML(depth);
                return xml;
            }
        }

    }
}
