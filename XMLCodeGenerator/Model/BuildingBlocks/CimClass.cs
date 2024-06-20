using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public sealed class CimClass: Element, ICimClass
    {
        public CimClass(ElementBlueprint blueprint) : base(blueprint) { }
    }
}
