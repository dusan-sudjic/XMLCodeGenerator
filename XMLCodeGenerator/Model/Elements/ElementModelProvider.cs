using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace XMLCodeGenerator.Model.Elements
{
    public static class ElementModelProvider
    {
        private static string path = "../../../Input/model.xml";
        private static List<ElementModel> ElementModels = new List<ElementModel>();
        private static Dictionary<string, FunctionModel> FunctionModels = new Dictionary<string, FunctionModel>();
        public static void LoadModel()
        {
            try
            {
                Dictionary<string, List<ElementModel>> elementTypes = new Dictionary<string, List<ElementModel>>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                XmlNodeList elementNodes = xmlDoc.SelectNodes("//Element");
                if (elementNodes != null)
                    foreach (XmlNode elementNode in elementNodes)
                        ElementModels.Add(new ElementModel(elementNode));

                XmlNodeList typeNodes = xmlDoc.SelectNodes("//Type");
                if (typeNodes != null)
                    foreach (XmlNode typeNode in typeNodes)
                    {
                        var name = typeNode.Attributes["Name"]?.InnerText;
                        var elementModels = new List<ElementModel>();
                        string[] elementNames = typeNode.InnerText.Trim().Split(',');
                        foreach (string elementName in elementNames)
                            elementModels.Add(GetElementModelByName(elementName));
                        elementTypes.Add(name, elementModels);
                    }

                foreach (var element in ElementModels)
                    element.SetContent(elementTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static List<FunctionModel> GetFunctions()
        {
            return FunctionModels.Values.OrderBy(x => x.FunctionName).ToList();
        }
        public static FunctionModel GetFunctionModelByName(string functionName)
        {
            return FunctionModels[functionName];
        }
        public static void RenameFunction(string functionName, string newName)
        {
            int calls = FunctionModels[functionName].CallsCounter;
            FunctionModels.Remove(functionName);
            FunctionModels.Add(newName, new FunctionModel(newName));
            FunctionModels[newName].CallsCounter = calls;
        }
        public static void AddFunctionCall(string functionName)
        {
            FunctionModel fm = FunctionModels[functionName];
            fm.CallsCounter++;
        }
        public static void DeleteFunctionCall(string functionName)
        {
            FunctionModel fm = FunctionModels[functionName];
            fm.CallsCounter--;
        }
        public static ElementModel GetElementModelByName(string name)
        {
            ElementModel model = ElementModels.FirstOrDefault(x => x.Name.Equals(name));
            if(model==null)
                return ElementModels.FirstOrDefault(x => name.Equals(x.FirstInContentBlockName));
            else 
                return model;
        }
        public static ElementModel GetElementModelByXMLElement(XmlElement xmlElement)
        {
            if (xmlElement.LocalName.Equals("Function"))
                if (!xmlElement.ParentNode.LocalName.Equals("FunctionDefinitions"))
                    return GetFunctionModelByName(xmlElement.GetAttribute("Name"));
            var list = ElementModels.Where(x => x.XMLName.Equals(xmlElement.LocalName)).ToList();
            if (list.Count == 1)
                return list[0];
            var list2 = list.Where(x => x.ContentBlocks.Count > 0 ? xmlElement.ChildNodes.Count > 0 : xmlElement.ChildNodes.Count == 0).ToList();
            if (list2.Count == 1)
                return list2[0];
            var list3 = list.Where(x => x.Attributes.All(a => xmlElement.GetAttributeNode(a.Name) != null)).ToList();
            if (list3.Count == 1)
                return list3[0];
            if (!FunctionModels.ContainsKey(xmlElement.GetAttribute("Name")))
                return null;
            return FunctionModels[xmlElement.GetAttribute("Name")];
        }
        public static ElementModel GetElementModelByFirstInContentBlockName(string name)
        {
            return ElementModels.FirstOrDefault(e => e.FirstInContentBlockName!=null && e.FirstInContentBlockName.Equals(name));
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
        public static void ResetFunctionDefinitions()
        {
            FunctionModels.Clear();
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
            return list;
        }
        public static List<ElementModel> GetModelsForNewChildElement(Element element)
        {
            List<ElementModel> ret = new List<ElementModel>();
            foreach (var block in element.Model.ContentBlocks)
            {
                if (block.MaxSize == -1)
                    ret.AddRange(block.ElementModels);
                else
                {
                    int counter = 0;
                    foreach (var elem in element.ChildElements)
                    {
                        if (block.ElementModels.Contains(elem.Model)
                            || (elem.Model is FunctionModel && block.ElementModels.Contains(ElementModelProvider.GetElementModelByName("Function"))))
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
            return ret;
        }
    }
}
