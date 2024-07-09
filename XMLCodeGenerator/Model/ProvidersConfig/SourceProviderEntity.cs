using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class SourceProviderEntity : ProviderElement
    {
        public SourceProviderEntity(string name) : base(name.Trim())
        {
            Attributes = new();
        }
        public List<SourceProviderAttribute> Attributes { get; set; }
    }
}
