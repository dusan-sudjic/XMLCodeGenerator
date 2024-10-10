﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
                    foreach (XmlNode node in entitityNodes)
                    {
                        string name = node.SelectSingleNode("Name")?.InnerText.Trim();
                        SourceProviderEntity entity = new SourceProviderEntity(name);
                        foreach (XmlNode attributeNode in node.SelectNodes("EntityAttribute"))
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
                MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
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
