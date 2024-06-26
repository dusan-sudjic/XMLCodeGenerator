using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace XMLCodeGenerator.Model
{
    public sealed class Element
    {
        public List<Element> ChildElements { get; set; }
        public List<string> AttributeValues { get; set; }
        public ElementModel Model { get; set; }
        public ContentBlockModel ParentContentBlock { get; set; }
        public Element() 
        {
            ChildElements = new List<Element>();
            AttributeValues = new List<string>();
        }
        public Element(ElementModel model, ContentBlockModel parentContentBlock = null)
        {
            Model = model;
            ParentContentBlock = parentContentBlock;
            AttributeValues = new List<string>();
            foreach (var attr in Model.Attributes)
                AttributeValues.Add(attr.DefaultValue);

            ChildElements = new();
            foreach (var block in Model.ContentBlocks)
                for (int i = 0; i < block.MinSize; i++)
                    ChildElements.Add(new Element(block.GetDefaultElement(), block));
            ParentContentBlock = parentContentBlock;
        }
        public XmlElement ToXmlNode(XmlDocument doc = null)
        {
            if (doc == null) doc = new XmlDocument();
            XmlElement node = doc.CreateElement(Model.XMLName);
            foreach (var attr in Model.Attributes)
                node.SetAttribute(attr.Name, AttributeValues[Model.Attributes.IndexOf(attr)]);
            appendChildNodes(this, node, doc);
            return node;
        }
        private void appendChildNodes(Element element, XmlNode node, XmlDocument doc)
        {
            foreach (var child in element.ChildElements)
            {
                if (child.Model.XMLName.Length > 0)
                    node.AppendChild(child.ToXmlNode(doc));
                else
                    appendChildNodes(child, node, doc);
            }
        }
        public static Element FromXmlElement(XmlElement xmlElement, ContentBlockModel parentBlock = null)
        {
            Element element = new Element();
            element.Model = ModelProvider.GetElementModelByXMLName(xmlElement.Name);
            element.ParentContentBlock = parentBlock;
            foreach(XmlAttribute attr in xmlElement.Attributes)
                element.AttributeValues.Add(attr.Value);
            foreach (XmlElement childXmlElement in xmlElement.ChildNodes)
            {
                ElementModel childModel = ModelProvider.GetElementModelByXMLName(childXmlElement.Name);
                Element childElement = FromXmlElement(childXmlElement, element.Model.SupportsChildModel(childModel));
                element.ChildElements.Add(childElement);
            }
            return element;
        }
        public override string ToString() { return Model.Name; }
    }

}
