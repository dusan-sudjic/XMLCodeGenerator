using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class ProviderWindow : Window, INotifyPropertyChanged
    {
        private bool tabs;
        public bool Tabs
        {
            get => tabs;
            set
            {
                if (value != tabs)
                {
                    tabs = value;
                    OnPropertyChanged();
                }
            }
        }
        public ListBox ValuesListBox { get; set; }
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
                        Tabs = false;
                        foreach (var c in MainWindow.CimProfileClasses)
                            ProviderElements.Add(c);
                        break; 
                    }
                case Model.Elements.InputType.CIM_PROFILE_PROPERTY: 
                    {
                        Tabs = false;
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
                        Tabs = false;
                        MultiSelect = true;
                        foreach (var c in MainWindow.SourceProviderEntities)
                            ProviderElements.Add(c);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ATTRIBUTE: 
                    {
                        Tabs = true;
                        string[] values = FindValuesFromCimClass(attribute.Element);
                        List<SourceProviderEntity> entities;
                        if (values != null)
                            entities = MainWindow.SourceProviderEntities.Where(c => values.Any(v => v.Equals(c.Name))).ToList();
                        else
                            entities = MainWindow.SourceProviderEntities;
                        TabControl tabControl = (TabControl)this.FindName("TabControl");
                        foreach(var e in entities)
                        {
                            ListBox lb = new ListBox();
                            lb.ItemsSource = e.Attributes;
                            TabItem item = new TabItem
                            {
                                Header = e.Name,
                                Content = lb
                            };
                            tabControl.Items.Add(item);
                        }
                        break; 
                    }
                default: break;
            }
            if (!Tabs)
            {
                ValuesListBox.ItemsSource = ProviderElements;
                ValuesListBox.SelectedIndex = MultiSelect ? -1 : 0;
                selectDefaultValues();
            }
            searchTextBox = (WatermarkTextBox)this.FindName("search");
            searchTextBox.Focus();
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
            if (!Tabs)
            {
                if (MultiSelect)
                {
                    if (ValuesListBox.SelectedItems.Count == 0)
                    {
                        System.Windows.MessageBox.Show("Please select values.");
                        return;
                    }
                    string selectedValue = "";
                    foreach (ProviderElement provider in ValuesListBox.SelectedItems)
                        selectedValue = selectedValue + provider.Name + (ValuesListBox.SelectedItems.IndexOf(provider) == ValuesListBox.SelectedItems.Count - 1 ? "" : ", ");
                    Attribute.Value = selectedValue;
                    this.DialogResult = true;
                }
                else if (ValuesListBox.SelectedItem != null)
                {
                    var provider = ValuesListBox.SelectedItem as ProviderElement;
                    Attribute.Value = provider.Name;
                    this.DialogResult = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an option.");
                }
            }
            else
            {
                TabControl tabControl = (TabControl)this.FindName("TabControl");
                var provider = (tabControl.SelectedContent as ListBox).SelectedItem as ProviderElement;
                if (provider == null)
                {
                    System.Windows.MessageBox.Show("Please select an option.");
                    return;
                }
                Attribute.Value = provider.Name;
                this.DialogResult = true;
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
        private string[] FindValuesFromCimClass(ElementViewModel vm)
        {
            if (vm.Parent == null)
                return null;
            if (vm.Parent.Name.Equals("CimClass"))
                return vm.Parent.Attributes[1].Value.Split(',').Select(v=>v.Trim()).ToArray();
            return FindValuesFromCimClass(vm.Parent);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
