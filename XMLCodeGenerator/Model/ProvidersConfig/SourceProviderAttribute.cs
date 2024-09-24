using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.ProvidersConfig
{
    public class SourceProviderAttribute : ProviderElement
    {
        public SourceProviderAttribute(string name) : base(name) { }
        public HashSet<SourceProviderEntity> IncludedInEntities { get; set; } = new();
        public string IncludedInEntitiesLabel
        {
            get {
                string ret = "";
                foreach (var ent in IncludedInEntities)
                    ret += ent.Name + ", ";
                return ret.Substring(0, ret.Length - 2);
            }
            set { }
        }
        public override string ToString()
        {
            return Name + " " + IncludedInEntitiesLabel;
        }
    }
}
