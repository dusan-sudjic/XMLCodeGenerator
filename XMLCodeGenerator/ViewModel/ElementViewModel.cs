using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class ElementViewModel
    {
        public Element Element { get; set; }
        public ElementViewModel(Element element)
        {
            Element = element;
        }
        public ElementViewModel() { }

    }
}
