using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public sealed class Else: Element, IElse
    {
        public Else(ElementBlueprint blueprint) : base(blueprint) { }
    }
}
