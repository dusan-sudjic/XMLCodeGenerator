using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;
using ValueType = XMLCodeGenerator.Model.BuildingBlocks.Abstractions.ValueType;
using Attribute = XMLCodeGenerator.Model.BuildingBlocks.Abstractions.Attribute;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class AttributeViewModel: INotifyPropertyChanged
    {
        public string Name { 
            get => Attribute.Name;
            set
            {
                if(value!= Attribute.Name)
                {
                    Attribute.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Value { 
            get => Attribute.Value;
            set
            {
                if(value!= Attribute.Value)
                {
                    Attribute.Value = value;
                    OnPropertyChanged();
                }
            }
        }
        public ValueType ValueType
        { 
            get => Attribute.ValueType;
            set
            {
                if(value!= Attribute.ValueType)
                {
                    Attribute.ValueType = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsRequired{ 
            get => Attribute.IsRequired;
            set
            {
                if(value!= Attribute.IsRequired)
                {
                    Attribute.IsRequired = value;
                    OnPropertyChanged();
                }
            }
        }
        public Attribute Attribute { get; set; }
        public AttributeViewModel(Attribute attribute)
        {
            Attribute = attribute;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
