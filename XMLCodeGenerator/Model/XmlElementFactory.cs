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
            if (doc == null)
            {
                doc = new XmlDocument();
                if (element.Model.XMLName.Length == 0)
                    return null;
            }
            XmlElement node = doc.CreateElement(element.Model.XMLName);
            foreach (var attr in element.Model.Attributes)
                node.SetAttribute(attr.Name, element.AttributeValues[element.Model.Attributes.IndexOf(attr)]);
            appendChildNodes(element, doc, node);
            return node;
        }
        private static void appendChildNodes(Element element, XmlDocument doc, XmlNode node)
        {
            foreach (var child in element.ChildElements)
            {
                if (child.Model.XMLName.Length > 0)
                    node.AppendChild(GetXmlElement(child, doc));
                else
                    appendChildNodes(child, doc, node);
            }
        }
        public static Element GetElement(XmlElement xmlElement, ContentBlockModel parentBlock = null)
        {
            Element element = new Element();
            element.Model = ModelProvider.GetElementModelByXMLElement(xmlElement);
            element.ParentContentBlock = parentBlock;
            foreach (XmlAttribute attr in xmlElement.Attributes)
                element.AttributeValues.Add(attr.Value);
            addChildren(xmlElement, element);
            return element;
        }

        private static void addChildren(XmlElement xmlElement, Element element)
        {
            foreach (XmlElement childXmlElement in xmlElement.ChildNodes)
            {
                ElementModel childModel = ModelProvider.GetElementModelByXMLElement(childXmlElement);
                ContentBlockModel parentContentBlock = element.Model.GetSuitableContentBlockForChildModel(childModel);
                if (parentContentBlock == null)
                {
                    ElementModel supportingModel = ModelProvider.GetWrapperModel(childModel);
                    Element supportingElement = new Element();
                    supportingElement.Model = supportingModel;
                    supportingElement.ParentContentBlock = element.Model.GetSuitableContentBlockForChildModel(supportingModel);
                    addChildren(xmlElement, supportingElement);
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
