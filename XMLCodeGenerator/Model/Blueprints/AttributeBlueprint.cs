using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;
using ValueType = XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType;

namespace XMLCodeGenerator.Model.Blueprints
{
    public class AttributeBlueprint
    {
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public ValueType ValueType { get; set; }
        public AttributeBlueprint(string name, bool required, ValueType type) 
        {
            Name = name;
            IsRequired = required;
            ValueType = type;
        }
        public string DefaultValue
        {
            get{
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
}
