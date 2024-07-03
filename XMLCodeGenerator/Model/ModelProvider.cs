using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XMLCodeGenerator.Model
{
    public static class ModelProvider
    {
        private static string path = "../../../Input/model.xml";
        private static List<ElementModel> ElementModels = new();
        private static Dictionary<string,FunctionModel> FunctionModels = new();
        public static void LoadModel()
        {
            try
            {
                Dictionary<string, List<ElementModel>> elementTypes = new();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                XmlNodeList elementNodes = xmlDoc.SelectNodes("//Element");
                if (elementNodes != null)
                    foreach (XmlNode elementNode in elementNodes)
                        ElementModels.Add(new ElementModel(elementNode));

                XmlNodeList typeNodes = xmlDoc.SelectNodes("//Type");
                if(typeNodes != null)
                    foreach (XmlNode typeNode in typeNodes)
                    {
                        var name = typeNode.Attributes["Name"]?.InnerText;
                        var elementModels = new List<ElementModel>();
                        string[] elementNames = typeNode.InnerText.Trim().Split(',');
                        foreach (string elementName in elementNames)
                            elementModels.Add(GetElementModelByName(elementName));
                        elementTypes.Add(name, elementModels);
                    }

                foreach(var element in ElementModels)
                    element.SetContent(elementTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static List<FunctionModel> GetFunctions()
        {
            return FunctionModels.Values.ToList();
        }
        public static FunctionModel GetFunctionModelByName(string functionName)
        {
            return FunctionModels[functionName];
        }
        public static ElementModel GetElementModelByName(string name)
        {
            return ElementModels.FirstOrDefault(x=>x.Name.Equals(name));
        }
        public static ElementModel GetElementModelByXMLElement(XmlElement xmlElement)
        {
            if(xmlElement.Name.Equals("Function"))
                if(!xmlElement.ParentNode.Name.Equals("FunctionDefinitions"))
                    return GetFunctionModelByName(xmlElement.GetAttribute("Name"));
            var list = ElementModels.Where(x => x.XMLName.Equals(xmlElement.Name)).ToList();
            if (list.Count == 1)
                return list[0];
            var list2 = list.Where(x=>x.ContentBlocks.Count>0 ? xmlElement.ChildNodes.Count>0 : xmlElement.ChildNodes.Count==0).ToList();
            if(list2.Count ==1)
                return list2[0];
            var list3 = list.Where(x => x.Attributes.All(a=>xmlElement.GetAttributeNode(a.Name) != null)).ToList();
            if(list3.Count == 1)
                return list3[0];
            var model = FunctionModels[xmlElement.GetAttribute("Name")];
            if (model == null)
                return null;
            return model;
        }
        public static void AddNewFunctionDefinition(string functionName)
        {
            FunctionModels.Add(functionName, new FunctionModel(functionName));
        }
        public static bool FunctionNameAlreadyInUse(string functionName)
        {
            return FunctionModels.ContainsKey(functionName);
        }
        public static void RemoveFunctionDefinition(string functionName)
        {
            FunctionModels.Remove(functionName);
        }
        public static ElementModel GetWrapperModel(ElementModel childModel)
        {
            return ElementModels.Where(x => x.XMLName.Length == 0).FirstOrDefault(x => x.GetSuitableContentBlockForChildModel(childModel) != null);
        }
        public static List<ElementModel> GetReplacableModelsForElement(Element element)
        {
            if (element.ParentContentBlock == null) 
                return null;
            if (element.ParentContentBlock.ElementModels.Count == 1) 
                return null;
            var list = element.ParentContentBlock.ElementModels.Where(e => !e.Name.Equals(element.Model.Name)).ToList();
            var functionCallModel = list.FirstOrDefault(x => x.Name.Equals("Function"));
            if (functionCallModel != null)
            {
                list.Remove(functionCallModel);
                list.AddRange(GetFunctions());
            }
            return list;
        }
        public static List<ElementModel> GetModelsForNewChildElement(Element element)
        {
            List<ElementModel> ret = new();
            foreach (var block in element.Model.ContentBlocks)
            {
                if (block.MaxSize == -1)
                    ret.AddRange(block.ElementModels);
                else
                {
                    int counter = 0;
                    foreach (var elem in element.ChildElements)
                    {
                        if (block.ElementModels.Contains(elem.Model))
                        {
                            counter++;
                            continue;
                        }
                        if (counter > 0) break;
                    }
                    if (counter < block.MaxSize)
                        ret.AddRange(block.ElementModels);
                }
            }
            var functionCallModel = ret.FirstOrDefault(x => x.Name.Equals("Function"));
            if (functionCallModel != null)
            {
                ret.Remove(functionCallModel);
                ret.AddRange(GetFunctions());
            }
            return ret;
        }
    }
}
