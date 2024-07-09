using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class ProviderReaderProperty : ProviderElement
    {
        private string ClassName { get; init; }
        private string Type { get; init; }
        public ProviderReaderProperty(string name, string type, string className) : base(name)
        {
            Type = type;
            ClassName = className;
        }
        public override string ToString()
        {
            return $"{Name} : {Type}";
        }
    }
}
