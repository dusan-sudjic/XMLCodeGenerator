using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using XMLCodeGenerator.ViewModel;
namespace XMLCodeGenerator.View.Attributes
{
    /// <summary>
    /// Interaction logic for AttributeContainer.xaml
    /// </summary>
    public partial class AttributeContainer : UserControl
    {
        public AttributeViewModel Attribute { get; set; }
        public string Required
        {
            get { return Attribute.IsRequired ? "*" : ""; }
            set { }
        }
        private bool _isTrue;
        public bool IsTrue { get { return Attribute.Value == "true"; } set { Attribute.Value = value ? "true" : "false"; } }
        public AttributeContainer()
        {
            InitializeComponent();
        }
        public AttributeContainer(AttributeViewModel attribute):this()
        {
            DataContext = this;
            Attribute = attribute;
            HandleValueType();
            //Border border = (Border)this.FindName("Border");
            //border.BorderBrush = Attribute.IsRequiredValueSet ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Red);
            //border.BorderThickness = Attribute.IsRequiredValueSet ? new Thickness(0) : new Thickness(1);
        }
        private void HandleValueType()
        {
            switch (Attribute.ValueType)
            {
                case Model.ValueType.BOOLEAN:
                    {
                        var stringVal = (TextBox)this.FindName("StringValue");
                        stringVal.Visibility = System.Windows.Visibility.Collapsed;
                        var intVal = (IntegerUpDown)this.FindName("IntegerValue");
                        intVal.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                case Model.ValueType.STRING:
                    {
                        var boolVal = (CheckBox)this.FindName("BoolValue");
                        boolVal.Visibility = System.Windows.Visibility.Collapsed;
                        var intVal = (IntegerUpDown)this.FindName("IntegerValue");
                        intVal.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                case Model.ValueType.INTEGER:
                    {
                        var boolVal = (CheckBox)this.FindName("BoolValue");
                        boolVal.Visibility = System.Windows.Visibility.Collapsed;
                        var stringVal = (TextBox)this.FindName("StringValue");
                        stringVal.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
            }
        }
    }
}
