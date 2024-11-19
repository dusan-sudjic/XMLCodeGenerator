using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class CimProfileProperty : ProviderElement
    {
        private string ClassName { get; set; }
        private string Type { get; set; }
        public CimProfileProperty(string name, string type, string className) : base(name)
        {
            Type = type;
            ClassName = className;
            DisplayedName = $"{ClassName}.{Name} : {Type}";
        }
        public override string ToString()
        {
            return $"{ClassName}.{Name}";
        }
    }
}
