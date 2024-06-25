using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.BuildingBlocks.Abstractions
{
    public class Attribute
    {
        public string Name { get; set; }
        public string Value{ get; set; }
        public ValueType ValueType { get; set; }
        public bool IsRequired { get; set; }
        public Attribute(string name, bool isRequired, ValueType type)
        {
            Name = name;
            IsRequired = isRequired;
            ValueType = type;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public enum ValueType
    {
        INTEGER, STRING, BOOLEAN
    }
}
