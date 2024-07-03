using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLCodeGenerator.Model
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
        public bool SupportsElementModel(ElementModel model)
        {
            if (model is FunctionModel)
                return ElementModels.Contains(ModelProvider.GetElementModelByName("Function"));
            return ElementModels.Contains(model);
        }
        public void SetContent(Dictionary<string, List<ElementModel>> elementTypes)
        {
            if (elementTypes.ContainsKey(ElementsString))
                ElementModels = elementTypes[ElementsString];
            else
                foreach(var el in ElementsString.Split(','))
                    ElementModels.Add(ModelProvider.GetElementModelByName(el));
        }
        public ElementModel GetDefaultElement()
        {
            return ElementModels[0];
        }
        public override string ToString()
        {
            return ElementsString;
        }
    }
}
