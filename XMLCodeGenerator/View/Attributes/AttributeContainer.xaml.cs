using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using XMLCodeGenerator.Model.Elements;
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
            ChooseButton.Visibility = attribute.InputType != InputType.USER_INPUT ? Visibility.Visible : Visibility.Collapsed;
        }
        private void OpenProvidersWindow_Click(object sender, RoutedEventArgs e)
        {
            ProviderWindow window = new ProviderWindow(Attribute.InputType);
            if (window.ShowDialog() == true)
            {
                Attribute.Value = window.SelectedValue;
            }
        }
        private void HandleValueType()
        {
            switch (Attribute.ValueType)
            {
                case Model.Elements.ValueType.BOOLEAN:
                    {
                        var stringVal = (TextBox)this.FindName("StringValue");
                        stringVal.Visibility = System.Windows.Visibility.Collapsed;
                        var intVal = (IntegerUpDown)this.FindName("IntegerValue");
                        intVal.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                case Model.Elements.ValueType.STRING:
                    {
                        var boolVal = (CheckBox)this.FindName("BoolValue");
                        boolVal.Visibility = System.Windows.Visibility.Collapsed;
                        var intVal = (IntegerUpDown)this.FindName("IntegerValue");
                        intVal.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                case Model.Elements.ValueType.INTEGER:
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
