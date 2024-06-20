using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model
{
    public static class BlueprintsProvider
    {
        private static string path = "../../../Input/model.xml";
        public static List<ElementBlueprint> Elements = new();

        public static void LoadModel()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList entitityNodes = xmlDoc.SelectNodes("//Element");

                if (entitityNodes != null)
                {
                    foreach (XmlNode elementNode in entitityNodes)
                    {
                        string name = elementNode.Attributes["Name"]?.InnerText;
                        string contentPattern = elementNode.Attributes["ContentPattern"]?.InnerText;
                        string _interface = elementNode.Attributes["Interface"]?.InnerText;
                        string pattern = contentPattern == null ? "" : contentPattern; 
                        ElementBlueprint blueprint = new ElementBlueprint(name, pattern, getInterface(_interface));
                        foreach (XmlNode attributeNode in elementNode.SelectNodes("Attribute"))
                        {
                            string aname = attributeNode.Attributes["Name"]?.InnerText;
                            bool isRequired= bool.Parse(attributeNode.Attributes["IsRequired"]?.InnerText);
                            string type = attributeNode.Attributes["ValueType"]?.InnerText;
                            AttributeBlueprint ablueprint = new AttributeBlueprint(aname, isRequired, (XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType)Enum.Parse(typeof(XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType), type));
                            blueprint.Attributes.Add(ablueprint);
                        }
                        Elements.Add(blueprint);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static List<ChildrenPattern> GetChildrenPatternsOfContentPattern(string contentPattern)
        {
            List<ChildrenPattern> ret = new();
            string[] patterns = contentPattern.Split(')');
            patterns.Select(c => c = c.Substring(1));
            foreach (string pattern in patterns)
            {
                if (pattern.Length == 0) continue;
                string _interface = pattern.Split('{')[0].Split('(')[1];
                int minSize = int.Parse(pattern.Split('{')[1].Split(',')[0]);
                int maxSize = pattern.Split('{')[1].Split(',')[1].Equals("}") ? -1 : int.Parse(pattern.Split('{')[1].Split(',')[1].Substring(0, 1));
                ChildrenPattern childsBlueprint = new ChildrenPattern(getInterface(_interface), minSize, maxSize);
                ret.Add(childsBlueprint);
            }
            return ret;
        }
        public static ElementBlueprint GetBlueprint(string name)
        {
            return Elements.FirstOrDefault(e => e.XML_Name.Equals(name));
        }
        public static ElementBlueprint GetDefaultByInterface(Type _interface)
        {
            return Elements.FirstOrDefault(e => _interface.IsAssignableFrom(e.Interface));
        }
        public static List<ElementBlueprint> GetAllByInterface(Type _interface)
        {
            return Elements.Where(e => _interface.IsAssignableFrom(e.Interface)).ToList();
        }
        private static Type getInterface(string _interface)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(_interface));
        }
    }
}
