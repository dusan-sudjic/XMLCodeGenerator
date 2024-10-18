using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
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
        }
        public void ReplaceElement()
        {
            bool keepingContent = false;
            AddChildElementWindow window = new AddChildElementWindow(Element, true);
            if (window.ShowDialog() == true)
            {
                if (!window.ElementPastedFromClipboard && window.SelectedElement.Model.SupportsContentOfElement(Element.Element))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to keep the content of "+Element.Name+"?", "Confirmation",
                                                  MessageBoxButton.YesNo, MessageBoxImage.Question);
                    keepingContent = result == MessageBoxResult.Yes;
                }
                var newElement = Element.ReplaceElement(window.SelectedElement, keepingContent);
                MainWindow.ScrollToElement(newElement);
            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Element.IsExtended = !Element.IsExtended;
        }
        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            DeleteElement();
        }
        private void DeleteElement()
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
            AddChildElement(Element);   
        }

        public static void AddChildElement(ElementViewModel element)
        {
            var supportedChildElements = ElementModelProvider.GetModelsForNewChildElement(element.Element).Where(e => e is not FunctionModel).ToList();
            if (!(MainWindow.Document.Clipboard != null && supportedChildElements.Any(m => m == MainWindow.Document.Clipboard.Model))
                && supportedChildElements.Count == 1)
            {
                var newElement = element.AddNewChildElement(new Element(supportedChildElements[0]));
                MainWindow.ScrollToElement(newElement);
                return;
            }

            AddChildElementWindow window = new AddChildElementWindow(element);
            if (window.ShowDialog() == true)
            {
                if (window.SelectedElement != null)
                {
                    var newElement = element.AddNewChildElement(window.SelectedElement);
                    MainWindow.ScrollToElement(newElement);
                }
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
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Document.Clipboard = Element.Element.Copy();
            DeleteElement();
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
