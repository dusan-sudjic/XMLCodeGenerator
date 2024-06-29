using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCodeGenerator.Model
{
    public sealed class AttributeModel
    {
        public string Name { get; set; }
        public ValueType ValueType { get; set; }
        public bool IsRequired { get; set; }
        public bool Editable {  get; set; }
        public AttributeModel(XmlNode node)
        {
            Name = node.Attributes["Name"]?.InnerText;
            IsRequired = bool.Parse(node.Attributes["IsRequired"]?.InnerText);
            ValueType = (ValueType)Enum.Parse(typeof(ValueType), node.Attributes["ValueType"]?.InnerText);
            var editableText = node.Attributes["Editable"]?.InnerText;
            Editable = editableText != null ? bool.Parse(editableText) : true;
        }
        public AttributeModel()
        {
            Name = "Name";
            ValueType = ValueType.STRING;
            IsRequired = true;
        }
        public override string ToString()
        {
            return Name;
        }
        public string DefaultValue
        {
            get
            {
                switch (ValueType)
                {
                    case ValueType.STRING: return "null";
                    case ValueType.INTEGER: return "0";
                    case ValueType.BOOLEAN: return "false";
                    default: return "unexpected value";
                }
            }
        }
    }
    public enum ValueType
    {
        INTEGER, STRING, BOOLEAN
    }
}
