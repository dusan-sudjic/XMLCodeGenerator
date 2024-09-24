using System.Runtime.CompilerServices;
using System.ComponentModel;
using XMLCodeGenerator.Model.Elements;

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
        private string _value;
        public string Value { 
            get => _value;
            set
            {
                if(value!= _value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        public Model.Elements.ValueType ValueType
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
        public Model.Elements.ValueMappingComponent ValueMappingComponent
        { 
            get => Attribute.ValueMappingComponent;
            set
            {
                if(value!= Attribute.ValueMappingComponent)
                {
                    Attribute.ValueMappingComponent = value;
                    OnPropertyChanged();
                }
            }
        }
        public InputType InputType
        { 
            get => Attribute.InputType;
            set
            {
                if(value!= Attribute.InputType)
                {
                    Attribute.InputType = value;
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
        public string Required { get => IsRequired ? "*" : ""; set { } }
        public bool IsTrue { get { return Value == "true"; } set { Value = value ? "true" : "false"; } }
        public AttributeModel Attribute { get; set; }
        public ElementViewModel Element { get; init; }
        public AttributeViewModel(AttributeModel attribute, string value, ElementViewModel element)
        {
            Attribute = attribute;
            Value = value;
            Element = element;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
