using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.View;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class DocumentViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<ElementViewModel> CimClasses { get; set; }
        public ObservableCollection<ElementViewModel> FunctionDefinitions { get; set; }
        public string OutputPath { get; set; }
        private bool _hasUnsavedChages;
        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChages;
            set
            {
                if (value != _hasUnsavedChages)
                {
                    _hasUnsavedChages = value;
                    OnPropertyChanged();
                }
            }
        }
        public DocumentViewModel()
        {
            CimClasses = new();
            FunctionDefinitions = new();
        }
        public void Reset()
        {
            CimClasses.Clear();
            FunctionDefinitions.Clear();
            HasUnsavedChanges = false;
            ElementModelProvider.ResetFunctionDefinitions();
            OutputPath = null;
        }
        public void LoadFromXmlDocument(string path)
        {
            Reset();
            OutputPath = path;
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNodeList functionDefinitionNodes = document.SelectNodes("//FunctionDefinitions/Function");
            if (functionDefinitionNodes != null)
            {
                foreach (XmlElement functionDefinitionNode in functionDefinitionNodes)
                {
                    Element el = XmlElementFactory.GetElement(functionDefinitionNode);
                    ElementModelProvider.AddNewFunctionDefinition(el.AttributeValues[0]);
                    ElementViewModel elemVM = new ElementViewModel(el);
                    FunctionDefinitions.Add(elemVM);
                }
            }
            XmlNodeList cimClassNodes = document.SelectNodes("//CimClass");
            if (cimClassNodes != null)
            {
                foreach (XmlElement cimClassNode in cimClassNodes)
                {
                    ElementViewModel elemVM = new ElementViewModel(XmlElementFactory.GetElement(cimClassNode));
                    CimClasses.Add(elemVM);
                }
            }
            HasUnsavedChanges = false;
        }
        public XmlDocument ExportToXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("Root");
            var functionDefinitions = xmlDoc.CreateElement("FunctionDefinitions");
            foreach (var functionDefinition in FunctionDefinitions)
                functionDefinitions.AppendChild(XmlElementFactory.GetXmlElement(functionDefinition.Element, xmlDoc));
            rootElement.AppendChild(functionDefinitions);
            xmlDoc.AppendChild(rootElement);
            var cimClasses = xmlDoc.CreateElement("CimClasses");
            foreach (var cimClass in CimClasses)
                cimClasses.AppendChild(XmlElementFactory.GetXmlElement(cimClass.Element, xmlDoc));
            rootElement.AppendChild(cimClasses);
            HasUnsavedChanges = false;
            return xmlDoc;
        }

        public void AddCimClass()
        {
            ElementModel bp = ElementModelProvider.GetElementModelByName("CimClass");
            ElementViewModel element = new ElementViewModel(new Element(bp));
            CimClasses.Add(element);
        }
        public void AddFunctionDefinition(string name)
        {
            ElementModel model = ElementModelProvider.GetElementModelByName("FunctionDefinition");
            Element element = new Element(model);
            element.AttributeValues[0] = name;
            ElementModelProvider.AddNewFunctionDefinition(name);
            ElementViewModel elementVM = new ElementViewModel(element);
            FunctionDefinitions.Add(elementVM);
            foreach (var c in CimClasses)
                c.SetReplacable();
        }
        public void RenameFunction(string oldName, string newName)
        {
            FunctionDefinitions.First(x => x.Attributes[0].Value.Equals(oldName)).Attributes[0].Value = newName;
            ElementModelProvider.RenameFunction(oldName, newName);
            foreach (var c in CimClasses)
                c.RenameFunction(oldName, newName);
        }
        public void RemoveFunctionDefinition(ElementViewModel functionDefinition)
        {
            FunctionDefinitions.Remove(functionDefinition);
            ElementModelProvider.RemoveFunctionDefinition(functionDefinition.Attributes[0].Value);
            foreach (var c in CimClasses)
                c.SetReplacable();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(!propertyName.Equals("HasUnsavedChanges"))
                HasUnsavedChanges = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
