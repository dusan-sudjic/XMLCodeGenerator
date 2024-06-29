using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model
{
    public static class XmlElementFactory
    {
        public static XmlElement GetXmlElement(Element element, XmlDocument doc = null)
        {
            if (doc == null) doc = new XmlDocument();
            XmlElement node = doc.CreateElement(element.Model.XMLName);
            foreach (var attr in element.Model.Attributes)
                node.SetAttribute(attr.Name, element.AttributeValues[element.Model.Attributes.IndexOf(attr)]);
            appendChildNodes(element, node, doc);
            return node;
        }
        private static void appendChildNodes(Element element, XmlNode node, XmlDocument doc)
        {
            foreach (var child in element.ChildElements)
            {
                if (child.Model.XMLName.Length > 0)
                    node.AppendChild(GetXmlElement(child, doc));
                else
                    appendChildNodes(child, node, doc);
            }
        }
        public static Element GetElement(XmlElement xmlElement, ContentBlockModel parentBlock = null)
        {
            Element element = new Element();
            element.Model = ModelProvider.GetElementModelByXMLName(xmlElement.Name);
            element.ParentContentBlock = parentBlock;
            foreach (XmlAttribute attr in xmlElement.Attributes)
                element.AttributeValues.Add(attr.Value);
            AddChildren(xmlElement, element);
            return element;
        }

        private static void AddChildren(XmlElement xmlElement, Element element)
        {
            foreach (XmlElement childXmlElement in xmlElement.ChildNodes)
            {
                ElementModel childModel = ModelProvider.GetElementModelByXMLName(childXmlElement.Name);
                ContentBlockModel parentContentBlock = element.Model.SupportsChildModel(childModel);
                if (parentContentBlock == null)
                {
                    ElementModel supportingModel = ModelProvider.SupportingElementModels.First(x => x.SupportsChildModel(childModel) != null);
                    Element supportingElement = new Element();
                    supportingElement.Model = supportingModel;
                    supportingElement.ParentContentBlock = element.Model.SupportsChildModel(supportingModel);
                    AddChildren(xmlElement, supportingElement);
                    element.ChildElements.Add(supportingElement);
                    break;
                }
                else
                {
                    Element childElement = GetElement(childXmlElement, parentContentBlock);
                    element.ChildElements.Add(childElement);
                }
            }
        }
    }
}
