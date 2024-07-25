using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.ViewModel;
namespace XMLCodeGenerator.View.Attributes
{
    public partial class AttributeContainer : UserControl
    {
        public AttributeViewModel Attribute { get => DataContext as AttributeViewModel; }
        public AttributeContainer()
        {
            InitializeComponent();
            DataContextChanged += AttributeContainer_DataContextChanged;
        }
        private void AttributeContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HandleValueType();
            ChooseButton.Visibility = Attribute.InputType != InputType.USER_INPUT ? Visibility.Visible : Visibility.Collapsed;
            AttributeName.Visibility = Attribute.Name.Length>0 ? Visibility.Visible : Visibility.Collapsed;
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
