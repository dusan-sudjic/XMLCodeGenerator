using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model.Elements
{
    public static class XmlElementFactory
    {
        public static XmlElement GetXmlElement(Element element, XmlDocument doc = null)
        {
            if (element.Model.XMLName.Equals("val"))
                return null;
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
            if (element.ChildElements.Count == 1)
            {
                if (element.ChildElements[0].Model.XMLName.Equals("val"))
                {
                    node.InnerText = element.ChildElements[0].AttributeValues[0];
                    return;
                }
            }
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
            element.Model = ElementModelProvider.GetElementModelByXMLElement(xmlElement);
            element.ParentContentBlock = parentBlock;
            foreach(var attrmodel in element.Model.Attributes)
                element.AttributeValues.Add(attrmodel.DefaultValue);
            foreach (XmlAttribute attr in xmlElement.Attributes)
                element.AttributeValues[element.Model.Attributes.IndexOf(element.Model.Attributes.FirstOrDefault(a=>a.Name.Equals(attr.Name)))] = attr.Value;

            addChildren(xmlElement, element);
            return element;
        }

        private static void addChildren(XmlElement xmlElement, Element element)
        {
            XmlNodeList nodes = xmlElement.ChildNodes;
            if (nodes.Count == 0) return;
            XmlText val = nodes[0] as XmlText;
            if (val!=null)
            {
                var valModel = ElementModelProvider.GetElementModelByName("Value");
                var valEl = new Element(valModel, element.Model.GetSuitableContentBlockForChildModel(valModel));
                valEl.AttributeValues[0] = val.InnerText;
                element.ChildElements.Add(valEl);
                return;
            }
            foreach (XmlElement childXmlElement in nodes)
            {
                ElementModel childModel = ElementModelProvider.GetElementModelByXMLElement(childXmlElement);
                ContentBlockModel parentContentBlock = element.Model.GetSuitableContentBlockForChildModel(childModel);
                if (parentContentBlock == null)
                {
                    ElementModel supportingModel = ElementModelProvider.GetWrapperModel(childModel);
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
