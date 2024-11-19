using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class AddChildElementWindow : Window
    {
        public bool ElementPastedFromClipboard { get; set; } = false;
        public bool SupportsFunctions { get; set; }
        public bool ClipboardNotEmpty { get; set; }
        public string PasteButtonLabel { get; set; }
        public Element CopiedElement { get; set; }
        public ElementViewModel Element { get; set; }
        public ListBox ElementsListBox {  get; set; }
        public ListBox FunctionsListBox {  get; set; }
        public ElementModel SelectedElementModel { get; set; }
        public Element SelectedElement { get; set; }
        WatermarkTextBox searchTextBox { get; set; }
        List<ElementModel> SupportedChildElements { get; set; }
        List<FunctionModel> SupportedFunctionCalls { get; set; }
        public AddChildElementWindow(ElementViewModel element, bool replacement = false)
        {
            InitializeComponent();
            Element = element;
            DataContext = this;
            ElementsListBox = (ListBox)this.FindName("listBox");
            FunctionsListBox = (ListBox)this.FindName("functionsListBox");
            CopiedElement = MainWindow.Document.Clipboard;
            ClipboardNotEmpty = CopiedElement != null;
            PasteButtonLabel = ClipboardNotEmpty ? "Paste "+CopiedElement.Name : "";
            if (!replacement)
                SupportedChildElements = ElementModelProvider.GetModelsForNewChildElement(element.Element).Where(e=>!(e is FunctionModel)).ToList();
            else
                SupportedChildElements = ElementModelProvider.GetReplacableModelsForElement(element.Element).Where(e => !(e is FunctionModel)).OrderBy(x=>x.Name).ToList();
            if (SupportedChildElements.Where(e => e.Name.Equals("Function")).ToList().Count > 0)
            {
                SupportsFunctions = true;
                SupportedFunctionCalls = ElementModelProvider.GetFunctions();
                SupportedChildElements.RemoveAll(x => x.Name.Equals("Function"));
            }
            if (SupportedChildElements == null)
                tab.SelectedIndex = 1;
            if (ClipboardNotEmpty && !SupportedChildElements.Any(m => m == CopiedElement.Model) && !(replacement && Element.Element.Model==CopiedElement.Model))
                ClipboardNotEmpty = false;
            ElementsListBox.ItemsSource = SupportedChildElements;
            FunctionsListBox.ItemsSource = SupportedFunctionCalls;
            ElementsListBox.SelectedIndex = 0;
            FunctionsListBox.SelectedIndex = 0;
            this.Title = replacement ? "Replace " + Element.XMLName: "Add child element to " + Element.XMLName;
            searchTextBox = (WatermarkTextBox)this.FindName("search");
            searchTextBox.Focus();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                OnDownArrowPressed();
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                OnUpArrowPressed();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                Select();
                e.Handled = true;
            }
        }

        private void OnDownArrowPressed()
        {
            if (tab.SelectedIndex == 0)
            {
                if (ElementsListBox.SelectedIndex == -1)
                {
                    ElementsListBox.SelectedIndex = 0;
                }
                else if (ElementsListBox.SelectedIndex == ElementsListBox.Items.Count - 1)
                {
                    ElementsListBox.SelectedIndex = 0;
                }
                else
                {
                    ElementsListBox.SelectedIndex++;
                }
                ElementsListBox.ScrollIntoView(ElementsListBox.SelectedItem);
            }
            else
            {
                if (FunctionsListBox.SelectedIndex == -1)
                {
                    FunctionsListBox.SelectedIndex = 0;
                }
                else if (FunctionsListBox.SelectedIndex == FunctionsListBox.Items.Count - 1)
                {
                    FunctionsListBox.SelectedIndex = 0;
                }
                else
                {
                    FunctionsListBox.SelectedIndex++;
                }
                FunctionsListBox.ScrollIntoView(FunctionsListBox.SelectedItem);
            }
        }

        private void OnUpArrowPressed()
        {
            if (tab.SelectedIndex == 1)
            {
                if (FunctionsListBox.SelectedIndex == -1)
                {
                    FunctionsListBox.SelectedIndex = FunctionsListBox.Items.Count - 1;
                }
                else if (FunctionsListBox.SelectedIndex == 0)
                {
                    FunctionsListBox.SelectedIndex = FunctionsListBox.Items.Count - 1;
                }
                else
                {
                    FunctionsListBox.SelectedIndex--;
                }
                FunctionsListBox.ScrollIntoView(FunctionsListBox.SelectedItem);
            }
            else
            {
                if (ElementsListBox.SelectedIndex == -1)
                {
                    ElementsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                }
                else if (ElementsListBox.SelectedIndex == 0)
                {
                    ElementsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                }
                else
                {
                    ElementsListBox.SelectedIndex--;
                }
                ElementsListBox.ScrollIntoView(ElementsListBox.SelectedItem);
            }
        }

        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }
        public void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (tab.SelectedIndex == 0)
            {
                if (ElementsListBox.SelectedItem != null)
                {
                    SelectedElement = new Element(ElementsListBox.SelectedItem as ElementModel);
                    this.DialogResult = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an option.");
                }
            }
            else
            {
                if (FunctionsListBox.SelectedItem != null)
                {
                    SelectedElement = new Element(FunctionsListBox.SelectedItem as ElementModel);
                    this.DialogResult = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an option.");
                }
            }
        }

        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ElementModel> newList = new List<ElementModel>();
            foreach(var s in SupportedChildElements)
            {
                if (s.Name.ToLower().Contains(searchTextBox.Text.ToLower()))
                    newList.Add(s);
            }
            ElementsListBox.ItemsSource = newList;
            if (ElementsListBox.SelectedIndex == -1) ElementsListBox.SelectedIndex = 0;

            if (!SupportsFunctions) return;
            List<ElementModel> newListFunctions = new List<ElementModel>();
            foreach (var s in SupportedFunctionCalls)
            {
                if (s.ToString().ToLower().Contains(searchTextBox.Text.ToLower()))
                    newListFunctions.Add(s);
            }
            FunctionsListBox.ItemsSource = newListFunctions;
            if (FunctionsListBox.SelectedIndex == -1) FunctionsListBox.SelectedIndex = 0;
        }

        private void PasteElement_Click(object sender, RoutedEventArgs e)
        {
            SelectedElement = MainWindow.Document.Clipboard;
            ElementPastedFromClipboard = true;
            this.DialogResult = true;
        }
    }
}
