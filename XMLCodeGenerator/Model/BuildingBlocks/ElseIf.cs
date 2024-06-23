using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public sealed class ElseIf : Element, IElseIf
    {
        public ElseIf(ElementBlueprint blueprint) : base(blueprint) { }
    }
}
