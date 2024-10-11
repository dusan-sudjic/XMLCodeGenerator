using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace XMLCodeGenerator.Model.Elements
{
    public sealed class Element
    {
        private string _name= null;
        private string _xmlName= null;
        public string Name {
            get => _name == null ? Model.Name : _name;
            set => _name = value;
        }
        public string XMLName
        {
            get => _xmlName == null ? Model.XMLName : _xmlName;
            set => _xmlName = value;
        }
        public List<Element> ChildElements { get; set; }
        public List<string> AttributeValues { get; set; }
        public ElementModel Model { get; set; }
        public ContentBlockModel ParentContentBlock { get; set; }
        public Element()
        {
            ChildElements = new List<Element>();
            AttributeValues = new List<string>();
        }
        public void setFirstInContentBlock()
        {
            if (Model.FirstInContentBlockName != null)
            {
                Name = Model.FirstInContentBlockName;
                XMLName = Model.FirstInContentBlockName;
            }
        }
        public void resetToModelsNames()
        {
            Name = Model.Name;
            XMLName = Model.XMLName;
        }
        public Element(ElementModel model, ContentBlockModel parentContentBlock = null)
        {
            Model = model;
            ParentContentBlock = parentContentBlock;
            AttributeValues = new List<string>();
            foreach (var attr in Model.Attributes)
                AttributeValues.Add(attr.DefaultValue);

            ChildElements = new();
            foreach (var block in Model.ContentBlocks)
                for (int i = 0; i < block.MinSize; i++)
                {
                    Element newElement = new Element(block.GetDefaultElementModel(), block);
                    if (i==0)
                        newElement.setFirstInContentBlock();
                    ChildElements.Add(newElement);
                }
            ParentContentBlock = parentContentBlock;
        }
        public Element Copy()
        {
            Element ret = new Element();
            ret.Model = Model;
            ret.ParentContentBlock = ParentContentBlock;
            ret.Name = Name;
            ret.XMLName = XMLName;
            foreach(var attr in AttributeValues)
                ret.AttributeValues.Add(attr);
            foreach (var child in ChildElements)
                ret.ChildElements.Add(child.Copy());
            return ret;
        }
        public override string ToString()
        {
            if (Model.Name == "Function")
                return AttributeValues[0];
            return Model.Name;
        }
    }

}
