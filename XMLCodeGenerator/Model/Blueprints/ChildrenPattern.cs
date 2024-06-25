using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.Blueprints
{
    public sealed class ChildrenPattern
    {
        public Type Interface {  get; set; }
        public int MinSize {  get; set; }
        public int MaxSize { get; set; }
        public ChildrenPattern(Type @interface, int minSize, int maxSize)
        {
            Interface = @interface;
            MinSize = minSize;
            MaxSize = maxSize;
        }
        public override string ToString()
        {
            return Interface.Name+"{"+MinSize+","+MaxSize+"}";
        }
    }
}
