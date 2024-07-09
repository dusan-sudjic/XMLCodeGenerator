using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class ProviderReaderProperty : ProviderElement
    {
        public string Type { get; set; }
        public ProviderReaderProperty(string name, string type) : base(name)
        {
            Type = type;
        }
        public override string ToString()
        {
            return $"{Name} : {Type}";
        }
    }
}
