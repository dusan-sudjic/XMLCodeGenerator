using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using XMLCodeGenerator.Model;
namespace XMLCodeGenerator.View.Attributes
{
    /// <summary>
    /// Interaction logic for AttributeContainer.xaml
    /// </summary>
    public partial class AttributeContainer : UserControl
    {
        public Model.Attribute Attribute { get; set; }
        public string Required
        {
            get { return Attribute.IsRequired ? "*" : ""; }
            set { }
        }
        private bool _isTrue;
        public bool IsTrue { get { return Attribute.Value == "true"; } set { Attribute.setValue(value ? "true" : "false"); } }
        public AttributeContainer()
        {
            InitializeComponent();
        }
        public AttributeContainer(Model.Attribute attribute, bool getValueAttribute = false):this()
        {
            DataContext = this;
            Attribute = attribute;
            HandleValueType();
            //Border border = (Border)this.FindName("Border");
            //border.BorderBrush = Attribute.IsRequiredValueSet ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Red);
            //border.BorderThickness = Attribute.IsRequiredValueSet ? new Thickness(0) : new Thickness(1);
            if (getValueAttribute)
            {
                var combo = (ComboBox)this.FindName("ComboBox");
                combo.ItemsSource = new string[] { "SourceName", "EntityName" };
                combo.SelectedIndex = 0;
                combo.Visibility = System.Windows.Visibility.Visible;
                var name = (TextBlock)this.FindName("AttributeName");
                name.Visibility = System.Windows.Visibility.Collapsed;
            }
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
