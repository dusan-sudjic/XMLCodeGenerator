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
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;
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
        public string SelectedOption { get; set; }
        WatermarkTextBox textBox { get; set; }
        List<string> SupportedChildElements { get; set; }
        public AddChildElementWindow(ElementViewModel element, bool replacement = false)
        {
            InitializeComponent();
            Element = element;
            DataContext = this;
            lb = (ListBox)this.FindName("listBox");
            if (!replacement)
                SupportedChildElements = BlueprintsProvider.GetBlueprintsForNewChildElement(element.Element).Select(e => e.XML_Name).ToList();
            else
                SupportedChildElements = BlueprintsProvider.GetReplacementBlueprintsForElement(element.Element).Select(e => e.XML_Name).ToList();
            lb.ItemsSource = SupportedChildElements;
            lb.SelectedIndex = 0;
            this.Title = "Add child element to " + Element.XML_Name;
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
            if (lb.SelectedIndex == -1)
            {
                lb.SelectedIndex = 0;
                return;
            }
            if(lb.SelectedIndex == lb.Items.Count - 1)
            {
                lb.SelectedIndex = 0;
                return;
            }
            lb.SelectedIndex++;
        }
        private void OnUpArrowPressed()
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
        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (lb.SelectedItem != null)
            {
                SelectedOption = lb.SelectedItem as string;
                this.DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an option.");
            }
        }

        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> newList = new();
            foreach(var s in SupportedChildElements)
            {
                if (s.ToLower().Contains(textBox.Text.ToLower()))
                    newList.Add(s);
            }
            lb.ItemsSource = newList;
            if (lb.SelectedIndex == -1) lb.SelectedIndex = 0;
        }
    }
}
