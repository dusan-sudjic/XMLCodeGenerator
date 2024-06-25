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
        public void SetContent()
        {
            if (ElementsString.StartsWith('*'))
                ElementModels = ModelProvider.ElementTypes.First(x => x.Name.Equals(ElementsString.Substring(1))).ElementModels;
            else
                foreach(var el in ElementsString.Split(','))
                    ElementModels.Add(ModelProvider.GetElementModel(el));
        }
        public ElementModel GetDefaultElement()
        {
            return ElementModels[0];
        }
    }
}
