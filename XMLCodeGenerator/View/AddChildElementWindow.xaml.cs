using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Interaction logic for AddChildElementWindow.xaml
    /// </summary>
    public partial class AddChildElementWindow : Window
    {
        public ElementViewModel Element { get; set; }
        public ListBox lb {  get; set; }
        public ListBox lbFunctions {  get; set; }
        public ElementModel SelectedElement { get; set; }
        WatermarkTextBox textBox { get; set; }
        List<ElementModel> SupportedChildElements { get; set; }
        List<ElementModel> SupportedFunctionCalls { get; set; }
        public AddChildElementWindow(ElementViewModel element, bool replacement = false)
        {
            InitializeComponent();
            Element = element;
            DataContext = this;
            lb = (ListBox)this.FindName("listBox");
            lbFunctions = (ListBox)this.FindName("functionsListBox");
            if (!replacement)
                SupportedChildElements = ModelProvider.GetModelsForNewChildElement(element.Element).Where(e=>e is not FunctionModel).ToList();
            else
                SupportedChildElements = ModelProvider.GetReplacableModelsForElement(element.Element).Where(e => e is not FunctionModel).ToList();
            if (!replacement)
                SupportedFunctionCalls = ModelProvider.GetModelsForNewChildElement(element.Element).Where(e => e is FunctionModel).ToList();
            else
                SupportedFunctionCalls = ModelProvider.GetReplacableModelsForElement(element.Element).Where(e => e is FunctionModel).ToList();
            if (SupportedChildElements == null)
                tab.SelectedIndex = 1;
            lb.ItemsSource = SupportedChildElements;
            lbFunctions.ItemsSource = SupportedFunctionCalls;
            lb.SelectedIndex = 0;
            lbFunctions.SelectedIndex = 0;
            this.Title = replacement ? "Replace " + Element.XML_Name: "Add child element to " + Element.XML_Name;
            textBox = (WatermarkTextBox)this.FindName("search");
            textBox.Focus();
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
                if (lb.SelectedIndex == -1)
                {
                    lb.SelectedIndex = 0;
                    return;
                }
                if (lb.SelectedIndex == lb.Items.Count - 1)
                {
                    lb.SelectedIndex = 0;
                    return;
                }
                lb.SelectedIndex++;
            }
            else
            {
                if (lbFunctions.SelectedIndex == -1)
                {
                    lbFunctions.SelectedIndex = 0;
                    return;
                }
                if (lbFunctions.SelectedIndex == lb.Items.Count - 1)
                {
                    lbFunctions.SelectedIndex = 0;
                    return;
                }
                lbFunctions.SelectedIndex++;
            }
        }
        private void OnUpArrowPressed()
        {
            if (tab.SelectedIndex == 1)
            {
                if (lbFunctions.SelectedIndex == -1)
                {
                    lbFunctions.SelectedIndex = lb.Items.Count - 1;
                    return;
                }
                if (lbFunctions.SelectedIndex == 0)
                {
                    lbFunctions.SelectedIndex = lb.Items.Count - 1;
                    return;
                }
                lbFunctions.SelectedIndex--;
            }
            else
            {
                if (lb.SelectedIndex == -1)
                {
                    lb.SelectedIndex = lb.Items.Count - 1;
                    return;
                }
                if (lb.SelectedIndex == 0)
                {
                    lb.SelectedIndex = lb.Items.Count - 1;
                    return;
                }
                lb.SelectedIndex--;
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
                if (lb.SelectedItem != null)
                {
                    SelectedElement = lb.SelectedItem as ElementModel;
                    this.DialogResult = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an option.");
                }
            }
            else
            {
                if (lbFunctions.SelectedItem != null)
                {
                    SelectedElement = lbFunctions.SelectedItem as ElementModel;
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
                if (s.Name.ToLower().Contains(textBox.Text.ToLower()))
                    newList.Add(s);
            }
            lb.ItemsSource = newList;
            if (lb.SelectedIndex == -1) lb.SelectedIndex = 0;

            List<ElementModel> newListFunctions = new();
            foreach (var s in SupportedFunctionCalls)
            {
                if (s.ToString().ToLower().Contains(textBox.Text.ToLower()))
                    newListFunctions.Add(s);
            }
            lbFunctions.ItemsSource = newListFunctions;
            if (lbFunctions.SelectedIndex == -1) lbFunctions.SelectedIndex = 0;
        }
    }
}
