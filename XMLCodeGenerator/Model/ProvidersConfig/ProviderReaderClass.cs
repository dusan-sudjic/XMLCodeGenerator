﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class ProviderReaderClass : ProviderElement
    {
        public string Namespace { get; set; }
        public string InheritedClass { get; set; }
        public List<ProviderReaderProperty> Properties { get; set; }
        public ProviderReaderClass(Type type) : base(type.Name)
        {
            InheritedClass = type.BaseType.Name;
            Namespace = type.Namespace;
            Properties = new();
            foreach (var property in type.GetProperties())
                Properties.Add(new ProviderReaderProperty(property.Name, property.PropertyType.Name));
        }
        public override string ToString()
        {
            return $"{Name} (inherits from {InheritedClass})";
        }
    }
}
