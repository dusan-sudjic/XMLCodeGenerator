using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model
{
    public static class ModelProvider
    {
        private static string path = "../../../Input/model.xml";
        public static List<ElementModel> ElementModels = new();
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
                Console.WriteLine("e");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static ElementModel GetElementModel(string name)
        {
            return ElementModels.First(x=>x.Name.Equals(name));
        }
        public static List<ElementModel> GetReplacableModelsForElement(Element element)
        {
            if (element.ParentContentBlock == null) return null;
            if (element.ParentContentBlock.ElementModels.Contains(element.Model) && element.ParentContentBlock.ElementModels.Count > 1)
                return element.ParentContentBlock.ElementModels.Where(e => !e.Name.Equals(element.Model.Name)).ToList();
            else return null;
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
                    {
                        ret.AddRange(block.ElementModels);
                    }
                }
            }
            return ret;
        }
    }
}
