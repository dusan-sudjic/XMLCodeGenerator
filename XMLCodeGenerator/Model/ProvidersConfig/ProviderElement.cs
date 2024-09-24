using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public abstract class ProviderElement
    {
        public string Name { get; set; }
        public string DisplayedName { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public ProviderElement(string name) { Name = name; DisplayedName = name; }
    }
}
