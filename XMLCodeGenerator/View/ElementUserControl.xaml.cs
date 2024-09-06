using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using XMLCodeGenerator.Behaviors;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class ElementUserControl : UserControl, INotifyPropertyChanged
    {
        private UIElement _draggedElement;
        public string XML_Name { get { return Element.XMLName; } set { } }
        private int _originalIndex;
        public ElementViewModel Element { get => DataContext as ElementViewModel; }
        public ElementUserControl()
        {
            InitializeComponent();
            DataContextChanged += ElementUserControl_DataContextChanged;
        }
        private void ElementUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            itemsControlChildren.ItemsSource = Element.ChildViewModels;
            SetButtons();
        }

        public void SetButtons()
        {
            var addButton = (Button)this.FindName("AddButton");
            if (Element.DefaultNewChild != null)
                addButton.ToolTip = "Add " + Element.DefaultNewChild;
            else
                addButton.ToolTip = "Add new element to " + Element.Name;
            var deleteButton = (Button)this.FindName("DeleteButton");
            deleteButton.ToolTip = "Delete " + Element.ToString();
            var replaceButton = (Button)this.FindName("ReplaceButton");
            replaceButton.ToolTip = "Replace " + Element.ToString();
            var toggleButton = (ToggleButton)this.FindName("ToggleButton");
            toggleButton.Content = Element.IsExtended ? "-" : "+";
            Border dock = (Border)this.FindName("Border");
            dock.HorizontalAlignment = Element.IsExtended ? HorizontalAlignment.Stretch: HorizontalAlignment.Left;
            dock.HorizontalAlignment = !Element.IsExtendable ? HorizontalAlignment.Stretch: dock.HorizontalAlignment;
        }
        public void ReplaceElement()
        {
            AddChildElementWindow window = new AddChildElementWindow(Element, true);
            if (window.ShowDialog() == true)
            {
                Element.ReplaceElement(window.SelectedElement);
                SetButtons();
            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Border dock = (Border)this.FindName("Border");
            if (!Element.IsExtended)
            {
                ((ToggleButton)sender).Content = "-";
                dock.HorizontalAlignment = HorizontalAlignment.Stretch;
                //dock.MaxWidth = 1220;
                Element.IsExtended = true;
            }
            else
            {
                ((ToggleButton)sender).Content = "+";
                dock.HorizontalAlignment = HorizontalAlignment.Left;
                Element.IsExtended = false;
            }
            SetButtons();
        }
        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            if (Element.Element.Name.Equals("FunctionDefinition"))
            {
                if (!Element.FunctionCalls.Split("c")[0].Trim().Equals("0"))
                {
                    MessageBox.Show("This function cant be deleted because its still used in document.");
                    return;
                }
                MainWindow.RemoveFunctionDefinition(Element);
            }
            else
            {
                if (Element.IsRemovable)
                    Element.DeleteElement();
                else
                    ReplaceElement();
            }
        }

        public void XMLPreview_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.BindElementToXMLPreview(Element);
        }
        private void AddChildElement_Click(object sender, RoutedEventArgs e)
        {
            if (Element.DefaultNewChild == null)
            {
                AddChildElementWindow window = new AddChildElementWindow(Element);
                if (window.ShowDialog() == true)
                    Element.AddNewChildElement(window.SelectedElement);
            }
            else
            {
                Element.AddNewChildElement(Element.DefaultNewChild);
            }
            SetButtons();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RenameFunction_Click(object sender, RoutedEventArgs e)
        {
            CreateNewFunctionWindow functionWindow = new CreateNewFunctionWindow(Element.Attributes[0].Value);
            if (functionWindow.ShowDialog() == true)
            {
                if (ElementModelProvider.FunctionNameAlreadyInUse(functionWindow.Name))
                {
                    MessageBox.Show("Function with name " + functionWindow.Name + " already exists.");
                    return;
                }
                MainWindow.RenameFunction(Element.Attributes[0].Value, functionWindow.Name);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            Element.MoveUp();
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            Element.MoveDown();
        }
    }

}
