using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model
{
    public class Attribute
    {
        public string Name { get; set; }
        private string _value;
        public string Value
        {
            get {  return _value; }
            set {
                if (setValue(value))
                {
                    _value = value;
                }
            } }
        public ValueType ValueType { get; private set; }
        public bool IsRequired { get; set; }
        public bool IsValueSet { get; set; }
        public Attribute(string name, bool isRequired, ValueType type)
        {
            Name = name;
            IsRequired = isRequired;
            ValueType = type;
            Value = null;
            if (type == ValueType.BOOLEAN) Value = "false";
            if( type == ValueType.INTEGER) Value = "0";
            IsValueSet = false;
        }
        public bool setValue(string value)
        {
            if (value == null) return false;
            if (!IsValueOfProperType(value)) return false;
            _value = value;
            IsValueSet = true;
            return true;
        }
        public bool IsRequiredValueSet
        {
            get { return IsRequired ? IsValueSet : true; }
        }
        private bool IsValueOfProperType(string value)
        {
            switch (ValueType)
            {
                case ValueType.BOOLEAN:
                    {
                        string boolPattern = @"^(?i:true|false)$";
                        if (!Regex.IsMatch(value, boolPattern)) return false;
                        break;
                    }
                case ValueType.INTEGER:
                    {
                        string intPattern = @"^[+-]?\d+$";
                        if (!Regex.IsMatch(value, intPattern)) return false;
                        break;
                    }
                default: return true;
            }
            return true;
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
