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
        public InputType InputType { get; set; }
        public bool IsRequired { get; set; }
        public bool Editable {  get; set; }
        private string defaultValue = null;
        public AttributeModel(XmlNode node)
        {
            Name = node.Attributes["Name"]?.InnerText;
            IsRequired = bool.Parse(node.Attributes["IsRequired"]?.InnerText);
            ValueType = (ValueType)Enum.Parse(typeof(ValueType), node.Attributes["ValueType"]?.InnerText);
            InputType = (InputType)Enum.Parse(typeof(InputType), node.Attributes["Input"]?.InnerText);
            var editableText = node.Attributes["Editable"]?.InnerText;
            Editable = editableText != null ? bool.Parse(editableText) : true;
        }
        private AttributeModel() { }
        public static AttributeModel CreateAttributeModelForFunctionCall(string functionName)
        {
            AttributeModel am = new AttributeModel();
            am.Name = "Name";
            am.ValueType = ValueType.STRING;
            am.IsRequired = true;
            am.InputType = InputType.USER_INPUT;
            am.DefaultValue = functionName;
            return am;
        }
        public override string ToString()
        {
            return Name;
        }
        public string DefaultValue
        {
            get
            {
                if(defaultValue!=null)
                    return defaultValue;
                switch (ValueType)
                {
                    case ValueType.STRING: return "null";
                    case ValueType.INTEGER: return "0";
                    case ValueType.BOOLEAN: return "false";
                    default: return "unexpected value";
                }
            }
            set
            {
                defaultValue = value;
            }
        }
    }
    public enum ValueType
    {
        INTEGER, STRING, BOOLEAN
    }
    public enum InputType
    {
        USER_INPUT, SOURCE_PROVIDER, PROVIDER_READER
    }
}
