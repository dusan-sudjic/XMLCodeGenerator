using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model.BuildingBlocks.Implementations
{
    public sealed class Case : Element, ICase
    {
        public Case(ElementBlueprint blueprint) : base(blueprint) { }
    }
}
