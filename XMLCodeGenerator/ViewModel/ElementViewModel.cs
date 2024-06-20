using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.ViewModel
{
    public abstract class ElementViewModel: INotifyPropertyChanged
    {
        public bool IsExtendable
        {
            get => Element.ContentPattern.Length > 0;
        }
        private bool _isExtended;
        public bool IsExtended
        {
            get=>_isExtended;
            set
            {
                if(value!=_isExtended)
                {
                    _isExtended = value;
                    OnPropertyChanged();
                }
            }
        }
        public abstract Element Element { get; set; }
        public ElementViewModel(Element element)
        {
            Element = element;
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
