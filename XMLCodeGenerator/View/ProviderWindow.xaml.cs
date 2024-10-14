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
        public List<ProviderElement> ProviderElements { get; set; } = new();
        public ObservableCollection<SourceProviderEntity> Entities { get; set; } = new();
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
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    filter();
                }
            }
        }

        private bool choosingAttribute = false;
        public bool ChoosingAttribute {
            get => choosingAttribute;
            set 
            {
                if (value != choosingAttribute)
                {
                    choosingAttribute = value;
                    OnPropertyChanged();
                }            
            } 
        }
        public AttributeViewModel Attribute { get; set; }
        public ProviderWindow(AttributeViewModel attribute)
        {
            InitializeComponent();
            DataContext = this;
            Attribute = attribute;
            MultiSelect = false;
            switch (attribute.InputType) 
            {
                case Model.Elements.InputType.CIM_PROFILE_CLASS: 
                    {
                        MainWindow.ProvidersViewModel.LoadCimProfile();
                        ProviderElements.AddRange(MainWindow.ProvidersViewModel.CimProfileClasses);
                        break; 
                    }
                case Model.Elements.InputType.CIM_PROFILE_PROPERTY: 
                    {
                        MainWindow.ProvidersViewModel.LoadCimProfile();
                        string className = attribute.Element.Parent.Attributes[0].Value;
                        CimProfileClass cl = MainWindow.ProvidersViewModel.CimProfileClasses.Where(c => c.Name == className).FirstOrDefault();
                        if (cl == null)
                        {
                            foreach (var c in MainWindow.ProvidersViewModel.CimProfileClasses)
                                ProviderElements.AddRange(c.Properties);
                            break;
                        }
                        ProviderElements.AddRange(cl.Properties);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ENTITY: 
                    {
                        MainWindow.ProvidersViewModel.LoadSourceProvider();
                        MultiSelect = true;
                        ProviderElements.AddRange(MainWindow.ProvidersViewModel.SourceProviderEntities);
                        break; 
                    }
                case Model.Elements.InputType.SOURCE_PROVIDER_ATTRIBUTE: 
                    {
                        MainWindow.ProvidersViewModel.LoadSourceProvider();
                        ChoosingAttribute = true;
                        string[] values = FindValuesFromCimClass(attribute.Element);
                        List<SourceProviderEntity> entities;
                        if (values != null)
                            entities = MainWindow.ProvidersViewModel.SourceProviderEntities.Where(c => values.Any(v => v.Equals(c.Name))).ToList();
                        else
                            entities = MainWindow.ProvidersViewModel.SourceProviderEntities;
                        if (entities.Any())
                        {
                            foreach(var ent in entities)
                            {
                                foreach(var attr in ent.Attributes)
                                {
                                    ProviderElement existingAttribute = ProviderElements.FirstOrDefault(a => a.Name.Equals(attr.Name));
                                    if (existingAttribute == null)
                                    {
                                        attr.IncludedInEntities.Add(ent);
                                        ProviderElements.Add(attr);
                                    }
                                    else
                                        (existingAttribute as SourceProviderAttribute).IncludedInEntities.Add(ent);
                                }
                            }
                        }
                        foreach (var e in entities)
                            Entities.Add(e);
                        break; 
                    }
                case Model.Elements.InputType.ENUMERATION:
                    {
                        MainWindow.ProvidersViewModel.LoadEnumerationMapping();
                        MultiSelect = false;
                        ProviderElements.AddRange(MainWindow.ProvidersViewModel.Enumerations);
                        break;
                    }
                default: break;
            }
            search.Focus();
            listBox.ItemsSource = ProviderElements;
            listBox.SelectionMode = MultiSelect ? SelectionMode.Multiple : SelectionMode.Single;
            selectDefaultValues();
        }
        private void selectDefaultValues()
        {
            listBox.SelectedIndex = MultiSelect ? -1 : 0;
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
                listBox.ScrollIntoView(listBox.SelectedItem);
                return;
            }
            if (listBox.SelectedIndex == listBox.Items.Count - 1)
            {
                listBox.SelectedIndex = 0;
                listBox.ScrollIntoView(listBox.SelectedItem);
                return;
            }
            listBox.SelectedIndex++;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
        private void OnUpArrowPressed()
        {
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                listBox.ScrollIntoView(listBox.SelectedItem);
                return;
            }
            if (listBox.SelectedIndex == 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                listBox.ScrollIntoView(listBox.SelectedItem);
                return;
            }
            listBox.SelectedIndex--;
            listBox.ScrollIntoView(listBox.SelectedItem);
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
            string parameter = ChoosingAttribute ? SearchText.ToLower() : search.Text.ToLower();
            List<ProviderElement> newList = ProviderElements
                .Where(s=> parameter.Split(" ").All(p=>s.ToString().ToLower().Contains(p)))
                .ToList();
            listBox.ItemsSource = newList;
            selectDefaultValues();
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
        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

    }
}
