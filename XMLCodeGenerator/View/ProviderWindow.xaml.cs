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
        public List<ProviderElement> ProviderElements { get; set; }
        public ObservableCollection<SourceProviderEntity> Entities { get; set; }
        private bool _multiSelect;
        public bool MultiSelect { 
            get => _multiSelect;
            set
            {
                if(value!=_multiSelect)
                {
                    _multiSelect = value;
                    OnPropertyChanged();
                }
            }
        }
        public AttributeViewModel Attribute { get; set; }
        public SourceProviderEntity SelectedEntity { get; set; }
        public bool ChoosingAttribute { get; set; }
        public ProviderWindow(AttributeViewModel attribute)
        {
            InitializeComponent();
            DataContext = this;
            ProviderElements = new();
            ChoosingAttribute = false;
            Attribute = attribute;
            search.Focus();
            MultiSelect = false;
            switch (attribute.InputType) 
            {
                case Model.Elements.InputType.CIM_PROFILE_CLASS: 
                    {
                        ProviderElements.AddRange(MainWindow.CimProfileClasses);
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
                        ProviderElements.AddRange(cl.Properties);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ENTITY: 
                    {
                        MultiSelect = true;
                        ProviderElements.AddRange(MainWindow.SourceProviderEntities);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ATTRIBUTE: 
                    {
                        ChoosingAttribute = true;
                        Entities = new();
                        string[] values = FindValuesFromCimClass(attribute.Element);
                        List<SourceProviderEntity> entities;
                        if (values != null)
                            entities = MainWindow.SourceProviderEntities.Where(c => values.Any(v => v.Equals(c.Name))).ToList();
                        else
                            entities = MainWindow.SourceProviderEntities;
                        foreach(var e in entities)
                            Entities.Add(e);
                        EntitiesComboBox.Focus();
                        break; 
                    }
                default: break;
            }
            listBox.ItemsSource = ProviderElements;
            listBox.SelectedIndex = MultiSelect ? -1 : 0;
            listBox.SelectionMode = MultiSelect ? SelectionMode.Multiple : SelectionMode.Single;
            selectDefaultValues();
        }
        private void selectDefaultValues()
        {
            string[] values = MultiSelect ? Attribute.Value.Split(',').Select(v=> v.Trim()).ToArray() : [Attribute.Value];
            foreach(var choice in listBox.Items)
            {
                ProviderElement pe = choice as ProviderElement;
                if (values.Any(v => v.Equals(pe.Name)))
                {
                    if (MultiSelect)
                    {
                        listBox.SelectedItems.Add(choice);
                    }
                    else
                    {
                        listBox.SelectedItem = choice;
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
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = 0;
                return;
            }
            if (listBox.SelectedIndex == listBox.Items.Count - 1)
            {
                listBox.SelectedIndex = 0;
                return;
            }
            listBox.SelectedIndex++;
        }
        private void OnUpArrowPressed()
        {
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                return;
            }
            if (listBox.SelectedIndex == 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                return;
            }
            listBox.SelectedIndex--;
        }
        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            if (MultiSelect)
            {
                if (listBox.SelectedItems.Count == 0)
                {
                    System.Windows.MessageBox.Show("Please select values.");
                    return;
                }
                string selectedValue = "";
                foreach (ProviderElement provider in listBox.SelectedItems)
                    selectedValue = selectedValue + provider.Name + (listBox.SelectedItems.IndexOf(provider) == listBox.SelectedItems.Count - 1 ? "" : ", ");
                Attribute.Value = selectedValue;
                this.DialogResult = true;
            }
            else if (listBox.SelectedItem != null)
            {
                Attribute.Value = (listBox.SelectedItem as ProviderElement).Name;
                this.DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an option.");
            }
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            filter();
        }

        private void filter()
        {
            List<ProviderElement> newList = ProviderElements.Where(s=>s.Name.ToLower().Contains(search.Text.ToLower())).ToList();
            listBox.ItemsSource = newList;
            if (listBox.SelectedIndex == -1) listBox.SelectedIndex = 0;
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

        private void EntitySelected(object sender, SelectionChangedEventArgs e)
        {
            ProviderElements.Clear();
            if (SelectedEntity == null)
                return;
            ProviderElements.AddRange(SelectedEntity.Attributes);
            filter();
            listBox.SelectedIndex = MultiSelect ? -1 : 0;
            selectDefaultValues();
            search.Focus();
        }
    }
}
