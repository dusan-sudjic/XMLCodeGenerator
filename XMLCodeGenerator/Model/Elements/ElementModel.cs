using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xceed.Wpf.Toolkit;

namespace XMLCodeGenerator.Model.Elements
{
    public class ElementModel
    {
        public string Name { get; set; }
        public string XMLName { get; set; }
        public string FirstInContentBlockName { get; private set; }
        public List<ContentBlockModel> ContentBlocks { get; set; }
        public List<AttributeModel> Attributes { get; set; }
        public string NamespacePrefix { get; set; } = null;
        public bool ClassMappingEnabled{ get; set; } = false;
        protected ElementModel() { }
        public ElementModel(XmlNode node)
        {
            ContentBlocks = new();
            Attributes = new();
            Name = node.Attributes["Name"]?.InnerText;
            XMLName = node.Attributes["XMLName"]?.InnerText;
            NamespacePrefix = node.Attributes["NamespacePrefix"]?.InnerText;
            var classMapping = node.Attributes["ClassMappingEnabled"]?.InnerText;
            if (classMapping != null)
                ClassMappingEnabled = bool.Parse(classMapping);
            FirstInContentBlockName = node.Attributes["FirstInContentBlockName"]?.InnerText;
            foreach (XmlNode attributeNode in node.SelectNodes("Attribute"))
                Attributes.Add(new AttributeModel(attributeNode));

            foreach (XmlNode contentBlock in node.SelectNodes("ContentBlock"))
                ContentBlocks.Add(new ContentBlockModel(contentBlock));
        }
        public virtual ContentBlockModel GetSuitableContentBlockForChildModel(ElementModel model)
        {
            if (model is FunctionModel functionModel)
                return ContentBlocks.Where(x => x.ElementModels.Contains(ElementModelProvider.GetElementModelByName("Function"))).ToList().FirstOrDefault();
            return ContentBlocks.Where(x => x.ElementModels.Contains(model)).ToList().FirstOrDefault();
        }
        public static string IncreaseNamespaceLevel(string ns)
        {
            string newns = "";
            int i = 0;
            while (i < ns.Length)
            {
                if (!Char.IsDigit(ns[i]))
                    newns += ns[i];
                else break;
                i++;
            }
            if(i== ns.Length)
                newns += "1";
            else
            {
                int number= int.Parse(ns.Substring(i)) + 1;
                newns+= number.ToString();
            }
            return newns;
        }
        public void SetContent(Dictionary<string, List<ElementModel>> elementTypes)
        {
            foreach (ContentBlockModel contentBlock in ContentBlocks)
                contentBlock.SetContent(elementTypes);
        }
        public bool SupportsContentOfElement(Element element)
        {
            int currentBlockIndex = 0;
            if (!ContentBlocks.Any()) return false;
            var currentContentBlock = ContentBlocks[currentBlockIndex];
            int elementsIncludedInCurrentContentBlock = 0;
            foreach(var child in element.ChildElements)
            {
                if (currentContentBlock.ElementModels.Contains(child.Model))
                {
                    elementsIncludedInCurrentContentBlock++;
                }
                else
                {
                    if(elementsIncludedInCurrentContentBlock<currentContentBlock.MinSize 
                        || (currentContentBlock.MaxSize==-1 || elementsIncludedInCurrentContentBlock > currentContentBlock.MaxSize))
                        return false;
                    currentBlockIndex++;
                    currentContentBlock = ContentBlocks[currentBlockIndex];
                    elementsIncludedInCurrentContentBlock = 0;
                    if (currentContentBlock == null) return false;
                }
            }
            if (elementsIncludedInCurrentContentBlock < currentContentBlock.MinSize
                        || (currentContentBlock.MaxSize != -1 && elementsIncludedInCurrentContentBlock > currentContentBlock.MaxSize))
                return false;
            for(int i = currentBlockIndex+1; i < ContentBlocks.Count; i++)
            {
                if (ContentBlocks[i].MinSize != 0)
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
