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
            if (elementTypes.ContainsKey(ElementsString))
                ElementModels = elementTypes[ElementsString];
            else
                foreach (var el in ElementsString.Split(','))
                    ElementModels.Add(ElementModelProvider.GetElementModelByName(el));
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
