using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model.Blueprints
{
    public sealed record ElementBlueprint
    {
        public Type Interface { get; set; }
        public List<AttributeBlueprint> Attributes { get; set; }
        public string XML_Name { get; init; }
        public string ContentPattern { get; init; }
        public ElementBlueprint(string xmlName, string contentPattern, Type _interface)
        {
            XML_Name = xmlName;
            ContentPattern = contentPattern;
            Attributes = new();
            Interface = _interface;
        }
        public override string ToString()
        {
            return XML_Name + " pattern";
        }
    }
}
