using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using XMLCodeGenerator.Commands;
using XMLCodeGenerator.Model.ProvidersConfig;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class ProvidersViewModel: INotifyPropertyChanged
    {
        public ICommand ImportEnumerationMappingCommand { get; set; }
        public ICommand ImportCimProfileCommand { get; set; }
        public ICommand ImportSourceProviderCommand { get; set; }

        public List<CimProfileClass> CimProfileClasses = new();
        public List<SourceProviderEntity> SourceProviderEntities = new();
        public List<ProviderElement> Enumerations = new();
        public string CimProfilePath = "";
        public string SourceProviderPath = "";
        public string EnumerationMappingPath = "";
        private bool _isCimProfileImported;
        public bool IsCimProfileImported
        {
            get => _isCimProfileImported;
            set
            {
                if (value != _isCimProfileImported)
                {
                    _isCimProfileImported = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isEnumerationMappingImported;
        public bool IsEnumerationMappingImported
        {
            get => _isEnumerationMappingImported;
            set
            {
                if (value != _isEnumerationMappingImported)
                {
                    _isEnumerationMappingImported = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isSourceProviderImported;
        public bool IsSourceProviderImported
        {
            get => _isSourceProviderImported;
            set
            {
                if (value != _isSourceProviderImported)
                {
                    _isSourceProviderImported = value;
                    OnPropertyChanged();
                }
            }
        }
        public ProvidersViewModel() 
        {
            IsCimProfileImported = false;
            IsSourceProviderImported = false;
            IsEnumerationMappingImported = false;

            ImportCimProfileCommand = new RelayCommand(ImportCimProfile);
            ImportSourceProviderCommand = new RelayCommand(ImportSourceProvider);
            ImportEnumerationMappingCommand = new RelayCommand(ImportEnumerationMapping);
        }

        public void LoadCimProfile()
        {
            try
            {
                CimProfileClasses.Clear();
                Assembly assembly = Assembly.LoadFrom(CimProfilePath);
                Type[] types = assembly.GetTypes();
                if (types.Length == 0)
                {
                    MessageBox.Show("0 classes found.");
                    return;
                }
                foreach (Type type in types)
                    if (!type.IsAbstract)
                        CimProfileClasses.Add(new CimProfileClass(type));
                IsCimProfileImported = true;
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(CimProfilePath))
                    return;
                MessageBox.Show($"Error loading classes from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UnloadCimProfile()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to unload cim profile?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CimProfilePath = null;
                CimProfileClasses.Clear();
                IsCimProfileImported = false;
            }
        }

        public void LoadSourceProvider()
        {
            SourceProviderEntities.Clear();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(SourceProviderPath);
                XmlNodeList entityNodes = xmlDoc.SelectNodes("//Entity");

                if (entityNodes != null && entityNodes.Count>0)
                {
                    foreach (XmlNode node in entityNodes)
                    {
                        string name = node.SelectSingleNode("Name")?.InnerText.Trim();
                        if (name == null)
                        {
                            MessageBox.Show("Invalid entity found in source provider. Try with other file.");
                            SourceProviderEntities.Clear();
                            return;
                        }
                        SourceProviderEntity entity = new SourceProviderEntity(name);
                        var attributeNodes = node.SelectNodes("EntityAttribute");
                        if (attributeNodes != null)
                            foreach (XmlNode attributeNode in attributeNodes)
                            {
                                var attributeName = attributeNode.Attributes["Name"]?.Value;
                                if(attributeName == null)
                                {
                                    MessageBox.Show("Invalid entity attribute found in source provider. Try with other file.");
                                    SourceProviderEntities.Clear();
                                    return;
                                }
                                entity.Attributes.Add(new SourceProviderAttribute(attributeName));
                            }
                        SourceProviderEntities.Add(entity);
                    }
                    IsSourceProviderImported = true;
                }
                else
                    MessageBox.Show("No <Entity> elements found in the XML document.");
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(SourceProviderPath))
                    return;
                MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
            }
        }
        public void UnloadSourceProvider()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to unload source provider?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SourceProviderPath = null;
                SourceProviderEntities.Clear();
                IsSourceProviderImported = false;
            }
        }

        public void LoadEnumerationMapping()
        {
            Enumerations.Clear();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(EnumerationMappingPath);
                XmlNodeList enumNodes = xmlDoc.SelectNodes("//Enumeration");

                if (enumNodes != null)
                {
                    foreach (XmlNode node in enumNodes)
                    {
                        string name = node.Attributes["name"]?.InnerText.Trim();
                        if(name !=null)
                            Enumerations.Add(new ProviderElement(name));
                    }
                    IsEnumerationMappingImported = true;
                }
                else
                    MessageBox.Show("No <Enumeration> elements found in the XML document.");
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(EnumerationMappingPath))
                    return;
                MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
            }
        }
        public void UnloadEnumerationMapping()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to unload enumeration mapping?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                EnumerationMappingPath = null;
                Enumerations.Clear();
                IsEnumerationMappingImported = false;
            }
        }
        private void ImportEnumerationMapping(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == false)
                return;
            EnumerationMappingPath = openFileDialog.FileName;
            LoadEnumerationMapping();
        }
        private void ImportSourceProvider(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == false)
                return;
            SourceProviderPath = openFileDialog.FileName;
            LoadSourceProvider();
        }
        private void ImportCimProfile(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DLL Files (*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == false)
                return;
            CimProfilePath = openFileDialog.FileName;
            LoadCimProfile();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
