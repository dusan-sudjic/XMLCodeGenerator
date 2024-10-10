using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class ElementUserControl : UserControl, INotifyPropertyChanged
    {
        public string XML_Name { get { return Element.XMLName; } set { } }
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
            addButton.ToolTip = "Add new element to " + Element.Name;
            var deleteButton = (Button)this.FindName("DeleteButton");
            deleteButton.ToolTip = "Delete " + Element.Name;
            var replaceButton = (Button)this.FindName("ReplaceButton");
            replaceButton.ToolTip = "Replace " + Element.Name;
            var toggleButton = (ToggleButton)this.FindName("ToggleButton");
            toggleButton.Content = Element.IsExtended ? "-" : "+";
            Border dock = (Border)this.FindName("Border");
            dock.HorizontalAlignment = Element.IsExtended ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
            dock.HorizontalAlignment = !Element.IsExtendable ? HorizontalAlignment.Stretch : dock.HorizontalAlignment;
        }
        public void ReplaceElement()
        {
            AddChildElementWindow window = new AddChildElementWindow(Element, true);
            if (window.ShowDialog() == true)
            {
                var newElement = Element.ReplaceElement(window.SelectedElement);
                SetButtons();
                MainWindow.ScrollToElement(newElement);
            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Border dock = (Border)this.FindName("Border");
            if (!Element.IsExtended)
            {
                ((ToggleButton)sender).Content = "-";
                dock.HorizontalAlignment = HorizontalAlignment.Stretch;
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
            AddChildElementWindow window = new AddChildElementWindow(Element);
            if (window.ShowDialog() == true)
            {
                ElementViewModel newElement;
                if (window.ElementPasted)
                    newElement = Element.AddNewChildElement(null, MainWindow.Document.Clipboard);
                else if (window.SelectedElement != null)
                    newElement = Element.AddNewChildElement(window.SelectedElement);
                else return;
                SetButtons();
                MainWindow.ScrollToElement(newElement);
            }
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
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Document.Clipboard = Element.Element.Copy();
        }

        private void MapToClassButton_Click(object sender, RoutedEventArgs e)
        {
            string mappingInterface = getMappingInterface();
            if (mappingInterface != null)
            {
                MappingClassesWindow window = new(Element, mappingInterface);
                window.ShowDialog();
            }
        }
        private string getMappingInterface()
        {
            switch (MainWindow.Document.CurrentlyDisplayedTab)
            {
                case 2: return "IMappingPreProcessorProcedure";
                case 3: return "IMappingRewritingProcedure";
                default: return "IOperator";
            }
        }
    }
}
