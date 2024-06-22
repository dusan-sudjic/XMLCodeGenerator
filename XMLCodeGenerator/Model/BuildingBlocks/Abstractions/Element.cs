using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XMLCodeGenerator.Model.Blueprints;

namespace XMLCodeGenerator.Model.BuildingBlocks.Abstractions
{
    public abstract class Element : IElement
    {
        public List<IElement> ChildElements { get; set; }
        public HashSet<Attribute> Attributes { get; set; }
        public string XML_Name { get; init; }
        public string ContentPattern { get; init; }
        public Element(ElementBlueprint blueprint)
        {
            XML_Name = blueprint.XML_Name;
            ContentPattern = blueprint.ContentPattern;
            Attributes = new();
            foreach (var attribute in blueprint.Attributes)
            {
                Attribute a = new Attribute(attribute.Name, attribute.IsRequired, attribute.ValueType);
                a.Value = attribute.DefaultValue;
                Attributes.Add(a);
            }
            ChildElements = new();
        }
        public override string ToString() { return XML_Name; }
    }

}
