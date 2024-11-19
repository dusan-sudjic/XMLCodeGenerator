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
        public static Dictionary<string, string> Namespaces = new Dictionary<string, string>();
        public static XmlElement GetXmlElement(Element element, XmlDocument doc = null, XmlNode parentXmlElement = null, string parentName = null)
        {
            if (element.XMLName.Equals("val"))
                return null;
            if (doc == null)
            {
                doc = new XmlDocument();
                if (element.XMLName.Length == 0)
                    return null;
            }
            string prefix = FindNamespacePrefix(parentXmlElement, parentName);
            XmlElement node = CreateNode(element, doc, prefix);
            foreach (var attr in element.Model.Attributes)
                node.SetAttribute(attr.Name, element.AttributeValues[element.Model.Attributes.IndexOf(attr)]);
            appendChildNodes(element, doc, node, element.Model.Name);
            return node;
        }

        private static XmlElement CreateNode(Element element, XmlDocument doc, string prefix)
        {
            XmlElement node = null;
            if (String.IsNullOrEmpty(prefix))
                node = doc.CreateElement(element.XMLName);
            else
                node = doc.CreateElement(prefix, element.XMLName, Namespaces[prefix]);
            return node;
        }

        private static string FindNamespacePrefix(XmlNode parentXmlElement, string parentName)
        {
            string prefix = null;
            if (parentXmlElement != null)
            {
                prefix = ElementModelProvider.GetElementModelByName(parentName).NamespacePrefix;
                if (prefix != null && prefix.Length > 0)
                {
                    if (parentXmlElement.Prefix.StartsWith(prefix))
                        prefix = ElementModel.IncreaseNamespaceLevel(parentXmlElement.Prefix);
                    if (!Namespaces.ContainsKey(prefix))
                        Namespaces.Add(prefix, "http://example.com/" + prefix);
                }
            }
            return prefix;
        }

        private static void appendChildNodes(Element element, XmlDocument doc, XmlNode node, string parentName)
        {
            if (element.ChildElements.Count == 1)
            {
                if (element.ChildElements[0].XMLName.Equals("val"))
                {
                    node.InnerText = element.ChildElements[0].AttributeValues[0];
                    return;
                }
            }
            foreach (var child in element.ChildElements)
            {
                if (child.XMLName.Length > 0)
                    node.AppendChild(GetXmlElement(child, doc, node, parentName));
                else
                    appendChildNodes(child, doc, node, element.Model.Name);
            }
        }
        public static Element GetElement(XmlElement xmlElement, ContentBlockModel parentBlock = null, ContentBlockModel previousContentBlock = null)
        {
            Element element = new Element();
            element.Model = ElementModelProvider.GetElementModelByXMLElement(xmlElement);
            if(element.Model==null && parentBlock != null && parentBlock != previousContentBlock)
            {
                element.Model = ElementModelProvider.GetElementModelByFirstInContentBlockName(xmlElement.LocalName);
                element.setFirstInContentBlock();
            }
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
                if (childModel == null)
                    childModel = ElementModelProvider.GetElementModelByFirstInContentBlockName(childXmlElement.LocalName);
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
                    ContentBlockModel previousContentBlock = null;
                    if(element.ChildElements.Count > 0)
                        previousContentBlock = element.ChildElements.Last().ParentContentBlock;
                    Element childElement = GetElement(childXmlElement, parentContentBlock, previousContentBlock);
                    element.ChildElements.Add(childElement);
                }
            }
        }
    }
}
