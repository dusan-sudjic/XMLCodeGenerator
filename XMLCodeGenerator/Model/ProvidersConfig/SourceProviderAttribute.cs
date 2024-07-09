using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class SourceProviderAttribute : ProviderElement
    {
        public string EntityName { get; init; }
        public SourceProviderAttribute(string name, string entityName) : base(name) 
        {
            EntityName = entityName;
        }
    }
}
