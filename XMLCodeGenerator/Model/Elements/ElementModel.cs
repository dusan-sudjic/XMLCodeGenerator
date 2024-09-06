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
        protected ElementModel() { }
        public ElementModel(XmlNode node)
        {
            ContentBlocks = new();
            Attributes = new();
            Name = node.Attributes["Name"]?.InnerText;
            XMLName = node.Attributes["XMLName"]?.InnerText;
            NamespacePrefix = node.Attributes["NamespacePrefix"]?.InnerText;
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
        public override string ToString()
        {
            return Name;
        }
    }
}
