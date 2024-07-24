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
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.ProvidersConfig;

namespace XMLCodeGenerator.View
{
    public partial class ProviderWindow : Window
    {
        public string SelectedValue { get; set; }
        public Model.Elements.InputType InputType { get; set; }
        public List<CimProfileClass> Classes { get; set; }
        public List<SourceProviderEntity> Entities { get; set; }
        public ObservableCollection<CimProfileClass> SearchResultsClasses { get; set; } = new();
        public ObservableCollection<CimProfileProperty> SearchResultsProperties { get; set; } = new();
        public ObservableCollection<SourceProviderEntity> SearchResultsEntities { get; set; } = new();
        public ObservableCollection<SourceProviderAttribute> SearchResultsAttributes { get; set; } = new();
        public ProviderWindow(Model.Elements.InputType type)
        {
            InitializeComponent();
            DataContext = this;
            InputType = type;
            Classes = MainWindow.CimProfileClasses;
            Entities = MainWindow.SourceProviderEntities;
            tabControl.SelectedIndex = type==Model.Elements.InputType.CIM_PROFILE ? 0 : 1;
            setupSearchResults();
        }
        private void setupSearchResults()
        {
            foreach (CimProfileClass c in Classes)
            {
                SearchResultsClasses.Add(c);
                foreach (var property in c.Properties)
                    SearchResultsProperties.Add(property);
            }
            foreach (SourceProviderEntity c in Entities)
            {
                SearchResultsEntities.Add(c);
                foreach (var attribute in c.Attributes)
                    SearchResultsAttributes.Add(attribute);
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = ((sender as Button).Content as ProviderElement).Name;
            this.DialogResult = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            search.Text = "";
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (tabControl2.SelectedIndex)
            {
                case 0:
                    {
                        SearchResultsClasses.Clear();
                        foreach(var c in Classes)
                        {
                            if(c.ToString().ToLower().Contains(search.Text.ToLower()))
                                SearchResultsClasses.Add(c);
                        }
                        return;
                    }
                case 1:
                    {
                        SearchResultsProperties.Clear();
                        foreach (var c in Classes)
                        {
                            foreach(var p in c.Properties)
                                if (p.ToString().ToLower().Contains(search.Text.ToLower()))
                                    SearchResultsProperties.Add(p);
                        }
                        return;
                    }
                case 2:
                    {
                        SearchResultsEntities.Clear();
                        foreach (var c in Entities)
                        {
                            if (c.ToString().ToLower().Contains(search.Text.ToLower()))
                                SearchResultsEntities.Add(c);
                        }
                        return;
                    }
                case 3:
                    {
                        SearchResultsAttributes.Clear();
                        foreach (var c in Entities)
                        {
                            foreach(var a in c.Attributes)
                                if (a.ToString().ToLower().Contains(search.Text.ToLower()))
                                    SearchResultsAttributes.Add(a);
                        }
                        return;
                    }
                case -1:
                    break;
            }
        }
    }
}
