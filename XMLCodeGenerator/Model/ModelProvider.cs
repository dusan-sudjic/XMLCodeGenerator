using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XMLCodeGenerator.Model
{
    public static class ModelProvider
    {
        private static string path = "../../../Input/model.xml";
        public static List<ElementModel> ElementModels = new();
        public static List<ElementModel> NoXmlElementModels { get { return ElementModels.Where(x => x.XMLName.Length == 0).ToList(); } }
        public static List<ElementType> ElementTypes = new();
        public static void LoadModel()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                XmlNodeList elementNodes = xmlDoc.SelectNodes("//Element");
                if (elementNodes != null)
                    foreach (XmlNode elementNode in elementNodes)
                        ElementModels.Add(new ElementModel(elementNode));

                XmlNodeList typeNodes = xmlDoc.SelectNodes("//Type");
                if(typeNodes != null)
                    foreach (XmlNode typeNode in typeNodes)
                        ElementTypes.Add(new ElementType(typeNode));

                foreach(var element in ElementModels)
                    element.SetContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static ElementModel GetElementModelByName(string name)
        {
            return ElementModels.FirstOrDefault(x=>x.Name.Equals(name));
        }
        public static string GetFunctionCallModelName(string functionName)
        {
            return "FunctionCall [" + functionName + "]";
        }
        public static ElementModel GetElementModelByXMLElement(XmlElement xmlElement)
        {
            var list = ElementModels.Where(x => x.XMLName.Equals(xmlElement.Name)).ToList();
            if (list.Count == 1)
                return list[0];
            var list2 = list.Where(x=>x.ContentBlocks.Count>0 ? xmlElement.ChildNodes.Count>0 : xmlElement.ChildNodes.Count==0).ToList();
            if(list2.Count ==1)
                return list2[0];
            var list3 = list.Where(x => x.Attributes.All(a=>xmlElement.GetAttributeNode(a.Name) != null)).ToList();
            if(list3.Count == 1)
                return list3[0];
            List<ElementModel> list4 = ElementModels.Where(m => m.Name.Equals(GetFunctionCallModelName(xmlElement.GetAttribute("Name")))).ToList();
            return list4[0];
        }
        public static void AddNewFunctionDefinition(Element functionDefinition)
        {
            ElementModel newModel = ElementModel.CreateFunctionCallModel(functionDefinition);
            ElementModels.Add(newModel);
        }
        public static void RemoveFunctionDefinition(Element functionDefinition)
        {
            ElementModel model = ElementModels.FirstOrDefault(f => f.Name.Equals(GetFunctionCallModelName(functionDefinition.AttributeValues[0])));
            if(model!= null)
                ElementModels.Remove(model);
        }
        public static List<ElementModel> GetReplacableModelsForElement(Element element)
        {
            if (element.ParentContentBlock == null) 
                return null;
            if (element.ParentContentBlock.ElementModels.Count == 1) 
                return null;
            var list = element.ParentContentBlock.ElementModels.Where(e => !e.Name.Equals(element.Model.GetModel().Name)).ToList();
            list.AddRange(ElementModels.Where(m => m.FunctionDefinition != null && element.ParentContentBlock.ElementModels.Contains(m.GetModel())).ToList());
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
                        if (block.ElementModels.Contains(elem.Model.GetModel()))
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
            if(ret.Count>0)
                ret.AddRange(ElementModels.Where(m => m.FunctionDefinition != null && ret.Contains(m.GetModel())).ToList());  //for functions
            return ret;
        }
    }
}
