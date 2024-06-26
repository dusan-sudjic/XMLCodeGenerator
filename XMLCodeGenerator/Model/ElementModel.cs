using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model
{
    public sealed class ElementModel
    {
        public string Name { get; set; }
        public string XMLName { get; set; }
        public List<ContentBlockModel> ContentBlocks { get; set; }
        public List<AttributeModel> Attributes { get; set; }
        public ElementModel(XmlNode node) 
        {
            ContentBlocks = new();
            Attributes = new();
            Name = node.Attributes["Name"]?.InnerText;
            XMLName = node.Attributes["XMLName"]?.InnerText;

            foreach (XmlNode attributeNode in node.SelectNodes("Attribute"))
                Attributes.Add(new AttributeModel(attributeNode));

            foreach (XmlNode contentBlock in node.SelectNodes("ContentBlock"))
                ContentBlocks.Add(new ContentBlockModel(contentBlock));
        }
        public ContentBlockModel SupportsChildModel(ElementModel model)
        {
            return ContentBlocks.Where(x=>x.ElementModels.Contains(model)).ToList().FirstOrDefault();
        }
        public void SetContent()
        {
            foreach(ContentBlockModel contentBlock in ContentBlocks)
                contentBlock.SetContent();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
