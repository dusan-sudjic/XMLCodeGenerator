using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model
{
    public class ElementType
    {
        public string Name {get; set;}
        public List<ElementModel> ElementModels { get; set;}
        public ElementType(XmlNode node) 
        {
            Name = node.Attributes["Name"]?.InnerText;
            ElementModels = new List<ElementModel>();
            string[] elementNames = node.InnerText.Trim().Split(',');
            foreach(string elementName in elementNames)
                ElementModels.Add(ModelProvider.GetElementModelByName(elementName));
        }
    }
}
