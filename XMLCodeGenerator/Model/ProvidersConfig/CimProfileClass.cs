using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class CimProfileClass : ProviderElement
    {
        public string Namespace { get; set; }
        public string InheritedClass { get; set; }
        public List<CimProfileProperty> Properties { get; set; }
        public CimProfileClass(Type type) : base(type.Name)
        {
            InheritedClass = type.BaseType.Name;
            Namespace = type.Namespace;
            Properties = new();
            foreach (var property in type.GetProperties())
                if(!property.Name.EndsWith("HasValue") && !(property.Name.StartsWith("Is") && property.Name.EndsWith("Mandatory")) && !property.Name.EndsWith("Prefix"))
                    Properties.Add(new CimProfileProperty(property.Name, property.PropertyType.Name, Name));
        }
        public override string ToString()
        {
            return $"{Name} (inherits from {InheritedClass})";
        }
    }
}
