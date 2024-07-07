using System.Runtime.CompilerServices;
using System.ComponentModel;
using XMLCodeGenerator.Model;

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
        public Model.ValueType ValueType
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
        public Model.InputType InputType
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
        public AttributeModel Attribute { get; set; }
        public AttributeViewModel(AttributeModel attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            MainWindow.HasUnsavedChanges = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
