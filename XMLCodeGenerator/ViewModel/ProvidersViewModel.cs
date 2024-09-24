using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using XMLCodeGenerator.Model.ProvidersConfig;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class ProvidersViewModel: INotifyPropertyChanged
    {
        public List<CimProfileClass> CimProfileClasses = new();
        public List<SourceProviderEntity> SourceProviderEntities = new();
        public string CimProfilePath = "";
        public string SourceProviderPath = "";
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
        private bool _isSourceProviderImported;
        public bool isSourceProviderImported
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
            isSourceProviderImported = false;
        }

        public void LoadCimProfile()
        {
            try
            {
                CimProfileClasses.Clear();
                Assembly assembly = Assembly.LoadFrom(CimProfilePath);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                    if (!type.IsAbstract)
                        CimProfileClasses.Add(new CimProfileClass(type));
                IsCimProfileImported = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadSourceProvider()
        {
            SourceProviderEntities.Clear();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(SourceProviderPath);
                XmlNodeList entitityNodes = xmlDoc.SelectNodes("//Entity");

                if (entitityNodes != null)
                {
                    foreach (XmlNode personNode in entitityNodes)
                    {
                        string name = personNode.SelectSingleNode("Name")?.InnerText.Trim();
                        SourceProviderEntity entity = new SourceProviderEntity(name);
                        foreach (XmlNode attributeNode in personNode.SelectNodes("EntityAttribute"))
                            entity.Attributes.Add(new SourceProviderAttribute(attributeNode.Attributes["Name"]?.Value));
                        SourceProviderEntities.Add(entity);
                    }
                    isSourceProviderImported = true;
                }
                else
                    MessageBox.Show("No <Entity> elements found in the XML document.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
