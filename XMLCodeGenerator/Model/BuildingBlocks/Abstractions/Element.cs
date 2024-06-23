using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using XMLCodeGenerator.Model.Blueprints;

namespace XMLCodeGenerator.Model.BuildingBlocks.Abstractions
{
    public abstract class Element : IElement
    {
        public List<IElement> ChildElements { get; set; }
        protected static string tab = "    ";
        public HashSet<Attribute> Attributes { get; set; }
        public string XML_Name { get; init; }
        public string ContentPattern { get; init; }
        public bool Translate { get; set; }
        public Element(ElementBlueprint blueprint)
        {
            XML_Name = blueprint.XML_Name;
            ContentPattern = blueprint.ContentPattern;
            Translate = blueprint.Translate;
            Attributes = new();
            foreach (var attribute in blueprint.Attributes)
            {
                Attribute a = new Attribute(attribute.Name, attribute.IsRequired, attribute.ValueType);
                a.Value = attribute.DefaultValue;
                Attributes.Add(a);
            }
            ChildElements = new();
        }
        public virtual string ToXML(int depth)
        {
            string xml = "";
            if (!Translate)
            {
                foreach (var el in ChildElements)
                    xml += el.ToXML(depth);
            }
            else
            {
                for (int i = 0; i < depth; i++)
                    xml += tab;
                xml += "[b]<" + XML_Name + "[/b]";
                foreach (var attr in Attributes)
                    xml += "  " + attr.Name + "=" + "[r]\"" + attr.Value + "\"[/r]";
                if (ContentPattern.Length > 0)
                {
                    xml += "[b]>[/b]\n";
                    foreach (var el in ChildElements)
                        xml += el.ToXML(depth + 1);
                    for (int i = 0; i < depth; i++)
                        xml += tab;
                    xml += "[b]</" + XML_Name + ">[/b]\n";
                }
                else xml += "[b]/>[/b]\n";
            }
            return xml;
        }
        public override string ToString() { return XML_Name; }
    }

}
