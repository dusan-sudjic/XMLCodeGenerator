using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLCodeGenerator.Model.Elements
{
    public sealed class ContentBlockModel
    {
        public int MaxSize { get; set; }
        public int MinSize { get; set; }
        public List<ElementModel> ElementModels { get; set; }
        public string ElementsString { get; set; }
        public ContentBlockModel(XmlNode node)
        {
            MaxSize = int.Parse(node.Attributes["MaxSize"]?.InnerText);
            MinSize = int.Parse(node.Attributes["MinSize"]?.InnerText);
            ElementsString = node.InnerText.Trim();
            ElementModels = new List<ElementModel>();
        }
        public void SetContent(Dictionary<string, List<ElementModel>> elementTypes)
        {
            foreach (var el in ElementsString.Split(','))
            {
                if (elementTypes.ContainsKey(el))
                    ElementModels.AddRange(elementTypes[el]);
                else
                    ElementModels.Add(ElementModelProvider.GetElementModelByName(el));
            }
        }
        public ElementModel GetDefaultElementModel()
        {
            return ElementModels[0];
        }
        public override string ToString()
        {
            return ElementsString;
        }
    }
}
