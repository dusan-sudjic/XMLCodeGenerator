using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model
{
    public static class XMLElementConverter
    {
        private static string tab = "    ";
        public static string ConvertElementToXML(IElement element, int depth = 1)
        {
            string xml = "[b]<" + element.XML_Name+"[/b]";
            foreach(var attr in  element.Attributes)
                xml += "  " + attr.Name + "="+"[r]\"" + attr.Value + "\"[/r]";
            if(element.ContentPattern.Length > 0) 
            {
                xml += "[b]>[/b]\n";
                foreach(var el in element.ChildElements)
                {
                    for (int i=0; i<depth; i++)
                        xml += tab;
                    xml += ConvertElementToXML(el,depth+1);
                }
                for (int i = 0; i < depth-1; i++)
                    xml += tab;
                xml += "[b]</" + element.XML_Name + ">[/b]\n";
            }
            else
                xml += "[b]/>[/b]\n";
            return xml;
        }
        public static IElement ConvertXMLToElement(string xml)
        {
            throw new NotImplementedException();
        }
    }
}
