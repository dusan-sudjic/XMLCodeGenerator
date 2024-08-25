using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class ProviderWindow : Window
    {
        public ListBox ValuesListBox { get; set; }
        public string SelectedValue { get; set; }
        WatermarkTextBox searchTextBox { get; set; }
        List<ProviderElement> ProviderElements { get; set; }
        public bool MultiSelect { get; set; } = false;
        public AttributeViewModel Attribute { get; set; }
        public ProviderWindow(AttributeViewModel attribute)
        {
            InitializeComponent();
            ProviderElements = new List<ProviderElement>();
            DataContext = this;
            ValuesListBox = (ListBox)this.FindName("listBox");
            Attribute = attribute;
            switch (attribute.InputType) 
            {
                case Model.Elements.InputType.CIM_PROFILE_CLASS: 
                    {
                        foreach (var c in MainWindow.CimProfileClasses)
                            ProviderElements.Add(c);
                        break; 
                    }
                case Model.Elements.InputType.CIM_PROFILE_PROPERTY: 
                    {
                        string className = attribute.Element.Parent.Attributes[0].Value;
                        CimProfileClass cl = MainWindow.CimProfileClasses.Where(c => c.Name == className).FirstOrDefault();
                        if (cl == null)
                        {
                            foreach (var c in MainWindow.CimProfileClasses)
                                foreach (var p in c.Properties)
                                    if (ProviderElements.Where(e => e.Name.Equals(p.Name)).ToList().Count == 0)
                                        ProviderElements.Add(p);
                            break;
                        }
                        foreach (var c in cl.Properties)
                            ProviderElements.Add(c);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ENTITY: 
                    { 
                        MultiSelect = true;
                        foreach (var c in MainWindow.SourceProviderEntities)
                            ProviderElements.Add(c);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ATTRIBUTE: 
                    {
                        string entityName = attribute.Element.Parent.Attributes[0].Value;
                        SourceProviderEntity cl = MainWindow.SourceProviderEntities.Where(c => c.Name == entityName).FirstOrDefault();
                        foreach (var c in cl.Attributes)
                            ProviderElements.Add(c);
                        break; 
                    }
                default: break;
            }
            ValuesListBox.ItemsSource = ProviderElements;
            ValuesListBox.SelectedIndex = MultiSelect ? -1 : 0;
            searchTextBox = (WatermarkTextBox)this.FindName("search");
            searchTextBox.Focus();
            selectDefaultValues();
        }
        private void selectDefaultValues()
        {
            string[] values = MultiSelect ? Attribute.Value.Split(',') : [Attribute.Value];
            values = values.Select(v => v.Trim()).ToArray();
            ValuesListBox.SelectionMode= MultiSelect?SelectionMode.Multiple:SelectionMode.Single;
            foreach(var choice in ValuesListBox.Items)
            {
                ProviderElement pe = choice as ProviderElement;
                if (values.Any(v => v.Equals(pe.Name)))
                {
                    if (MultiSelect)
                    {
                        ValuesListBox.SelectedItems.Add(choice);
                    }
                    else
                    {
                        ValuesListBox.SelectedItem = choice;
                        return;
                    }
                }
            }
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
            if (ValuesListBox.SelectedIndex == -1)
            {
                ValuesListBox.SelectedIndex = 0;
                return;
            }
            if (ValuesListBox.SelectedIndex == ValuesListBox.Items.Count - 1)
            {
                ValuesListBox.SelectedIndex = 0;
                return;
            }
            ValuesListBox.SelectedIndex++;
        }
        private void OnUpArrowPressed()
        {
            if (ValuesListBox.SelectedIndex == -1)
            {
                ValuesListBox.SelectedIndex = ValuesListBox.Items.Count - 1;
                return;
            }
            if (ValuesListBox.SelectedIndex == 0)
            {
                ValuesListBox.SelectedIndex = ValuesListBox.Items.Count - 1;
                return;
            }
            ValuesListBox.SelectedIndex--;
        }
        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (MultiSelect)
            {
                if (ValuesListBox.SelectedItems.Count == 0)
                {
                    System.Windows.MessageBox.Show("Please select values.");
                    return;
                }
                SelectedValue = "";
                foreach(ProviderElement provider in ValuesListBox.SelectedItems)
                    SelectedValue = SelectedValue + provider.Name + (ValuesListBox.SelectedItems.IndexOf(provider)==ValuesListBox.SelectedItems.Count-1 ? "" : ", ");
                Attribute.Value = SelectedValue;
                this.DialogResult = true;
            }
            else if (ValuesListBox.SelectedItem != null)
            {
                var provider = ValuesListBox.SelectedItem as ProviderElement;
                SelectedValue = provider.Name;
                Attribute.Value = SelectedValue;
                this.DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an option.");
            }
        }

        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ProviderElement> newList = new();
            foreach (var s in ProviderElements)
            {
                if (s.Name.ToLower().Contains(searchTextBox.Text.ToLower()))
                    newList.Add(s);
            }
            ValuesListBox.ItemsSource = newList;
            if (ValuesListBox.SelectedIndex == -1) ValuesListBox.SelectedIndex = 0;
        }
    }
}
