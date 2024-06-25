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
        public static List<ElementBlueprint> Blueprints = new();

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
                        string translateText = elementNode.Attributes["Translate"]?.InnerText;
                        bool translate = translateText != null ? translateText.Equals("true") : true;
                        string _interface = elementNode.Attributes["Interface"]?.InnerText;
                        string pattern = contentPattern == null ? "" : contentPattern; 
                        ElementBlueprint blueprint = new ElementBlueprint(name, pattern, getInterface(_interface), translate);
                        foreach (XmlNode attributeNode in elementNode.SelectNodes("Attribute"))
                        {
                            string aname = attributeNode.Attributes["Name"]?.InnerText;
                            bool isRequired= bool.Parse(attributeNode.Attributes["IsRequired"]?.InnerText);
                            string type = attributeNode.Attributes["ValueType"]?.InnerText;
                            AttributeBlueprint ablueprint = new AttributeBlueprint(aname, isRequired, (XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType)Enum.Parse(typeof(XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType), type));
                            blueprint.Attributes.Add(ablueprint);
                        }
                        Blueprints.Add(blueprint);
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
        public static bool ElementMatchesPattern(IElement element)
        {
            int index = 0;
            List<ChildrenPattern> patterns = GetChildrenPatternsOfContentPattern(element.ContentPattern);
            foreach (ChildrenPattern pattern in patterns)
            {
                int counter = 0;
                while (index<element.ChildElements.Count)
                {
                    IElement child = element.ChildElements[index];
                    if (!pattern.Interface.IsAssignableFrom(child.GetType()))
                        break;
                    counter++;
                    index++;
                }
                if (counter < pattern.MinSize || (pattern.MaxSize!=-1 && counter > pattern.MaxSize))
                    return false;
            }
            return index==element.ChildElements.Count;
        }
        public static List<ElementBlueprint> GetReplacementBlueprintsForElement(IElement element)
        {
            return Blueprints.Where(e => e.Interface.IsAssignableFrom(element.GetType())).ToList().Where(bp=>!bp.XML_Name.Equals(element.XML_Name)).ToList();
        }
        public static List<ElementBlueprint> GetBlueprintsForNewChildElement(IElement element)
        {
            List<ElementBlueprint> ret = new();
            List<ChildrenPattern> patterns = GetChildrenPatternsOfContentPattern(element.ContentPattern);
            foreach (var pattern in patterns)
            {
                if (pattern.MaxSize == -1)
                    ret.AddRange(GetAllByInterface(pattern.Interface));
                else
                {
                    int counter = 0;
                    foreach(var elem in element.ChildElements)
                    {
                        if (pattern.Interface.IsAssignableFrom(elem.GetType())) 
                        {
                            counter++;
                            continue;
                        }
                        if (counter > 0) break;
                    }
                    if(counter < pattern.MaxSize)
                    {
                        ret.AddRange(GetAllByInterface(pattern.Interface));
                    }
                }
            }
            return ret;
        }
        public static ElementBlueprint GetBlueprint(string name)
        {
            return Blueprints.FirstOrDefault(e => e.XML_Name.Equals(name));
        }
        public static ElementBlueprint GetDefaultByInterface(Type _interface)
        {
            return Blueprints.FirstOrDefault(e => _interface.IsAssignableFrom(e.Interface));
        }
        public static List<ElementBlueprint> GetAllByInterface(Type _interface)
        {
            return Blueprints.Where(e => _interface.IsAssignableFrom(e.Interface)).ToList();
        }
        private static Type getInterface(string _interface)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(_interface));
        }
    }
}
