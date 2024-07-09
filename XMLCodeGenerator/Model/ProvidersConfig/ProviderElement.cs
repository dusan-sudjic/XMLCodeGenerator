using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public abstract class ProviderElement : IComparable<ProviderElement>
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public int CompareTo(ProviderElement? other)
        {
            return this.ToString().CompareTo(other.ToString());
        }
        public ProviderElement(string name) { Name = name; }
    }
}
