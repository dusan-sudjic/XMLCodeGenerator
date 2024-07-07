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
using XMLCodeGenerator.Model;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class AddChildElementWindow : Window
    {
        public bool SupportsFunctions { get; set; }
        public ElementViewModel Element { get; set; }
        public ListBox ElementsListBox {  get; set; }
        public ListBox FunctionsListBox {  get; set; }
        public ElementModel SelectedElement { get; set; }
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
            if (!replacement)
                SupportedChildElements = ModelProvider.GetModelsForNewChildElement(element.Element).Where(e=>e is not FunctionModel).ToList();
            else
                SupportedChildElements = ModelProvider.GetReplacableModelsForElement(element.Element).Where(e => e is not FunctionModel).OrderBy(x=>x.Name).ToList();
            if (SupportedChildElements.Where(e => e.Name.Equals("Function")).ToList().Count > 0)
            {
                SupportsFunctions = true;
                SupportedFunctionCalls = ModelProvider.GetFunctions();
                SupportedChildElements.RemoveAll(x => x.Name.Equals("Function"));
            }
            if (SupportedChildElements == null)
                tab.SelectedIndex = 1;
            ElementsListBox.ItemsSource = SupportedChildElements;
            FunctionsListBox.ItemsSource = SupportedFunctionCalls;
            ElementsListBox.SelectedIndex = 0;
            FunctionsListBox.SelectedIndex = 0;
            this.Title = replacement ? "Replace " + Element.XML_Name: "Add child element to " + Element.XML_Name;
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
                    return;
                }
                if (ElementsListBox.SelectedIndex == ElementsListBox.Items.Count - 1)
                {
                    ElementsListBox.SelectedIndex = 0;
                    return;
                }
                ElementsListBox.SelectedIndex++;
            }
            else
            {
                if (FunctionsListBox.SelectedIndex == -1)
                {
                    FunctionsListBox.SelectedIndex = 0;
                    return;
                }
                if (FunctionsListBox.SelectedIndex == ElementsListBox.Items.Count - 1)
                {
                    FunctionsListBox.SelectedIndex = 0;
                    return;
                }
                FunctionsListBox.SelectedIndex++;
            }
        }
        private void OnUpArrowPressed()
        {
            if (tab.SelectedIndex == 1)
            {
                if (FunctionsListBox.SelectedIndex == -1)
                {
                    FunctionsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                    return;
                }
                if (FunctionsListBox.SelectedIndex == 0)
                {
                    FunctionsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                    return;
                }
                FunctionsListBox.SelectedIndex--;
            }
            else
            {
                if (ElementsListBox.SelectedIndex == -1)
                {
                    ElementsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                    return;
                }
                if (ElementsListBox.SelectedIndex == 0)
                {
                    ElementsListBox.SelectedIndex = ElementsListBox.Items.Count - 1;
                    return;
                }
                ElementsListBox.SelectedIndex--;
            }
        }
        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (tab.SelectedIndex == 0)
            {
                if (ElementsListBox.SelectedItem != null)
                {
                    SelectedElement = ElementsListBox.SelectedItem as ElementModel;
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
                    SelectedElement = FunctionsListBox.SelectedItem as ElementModel;
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
            List<ElementModel> newList = new();
            foreach(var s in SupportedChildElements)
            {
                if (s.Name.ToLower().Contains(searchTextBox.Text.ToLower()))
                    newList.Add(s);
            }
            ElementsListBox.ItemsSource = newList;
            if (ElementsListBox.SelectedIndex == -1) ElementsListBox.SelectedIndex = 0;

            List<ElementModel> newListFunctions = new();
            foreach (var s in SupportedFunctionCalls)
            {
                if (s.ToString().ToLower().Contains(searchTextBox.Text.ToLower()))
                    newListFunctions.Add(s);
            }
            FunctionsListBox.ItemsSource = newListFunctions;
            if (FunctionsListBox.SelectedIndex == -1) FunctionsListBox.SelectedIndex = 0;
        }
    }
}
