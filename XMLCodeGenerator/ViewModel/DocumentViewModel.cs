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
    public sealed class DocumentViewModel : INotifyPropertyChanged
    {
        public ElementViewModel CimClasses { get; set; }
        public ElementViewModel FunctionDefinitions { get; set; }
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
        public DocumentViewModel() { }
        public void setup()
        {
            CimClasses = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("CimClasses")));
            FunctionDefinitions = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("FunctionDefinitions")));
            HasUnsavedChanges = false;
        }
        public void Reset()
        {
            CimClasses.Element.ChildElements.Clear();
            CimClasses.ChildViewModels.Clear();
            FunctionDefinitions.ChildViewModels.Clear();
            FunctionDefinitions.Element.ChildElements.Clear();
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
                    el.ParentContentBlock = FunctionDefinitions.Element.Model.ContentBlocks[0];
                    ElementModelProvider.AddNewFunctionDefinition(el.AttributeValues[0]);
                    FunctionDefinitions.Element.ChildElements.Add(el);
                    FunctionDefinitions.ChildViewModels.Add(new ElementViewModel(el, FunctionDefinitions));
                    FunctionDefinitions.SetRemovableForChildren();
                }
            }
            XmlNodeList cimClassNodes = document.SelectNodes("//CimClass");
            if (cimClassNodes != null)
            {
                foreach (XmlElement cimClassNode in cimClassNodes)
                {
                    Element el = XmlElementFactory.GetElement(cimClassNode);
                    el.ParentContentBlock = CimClasses.Element.Model.ContentBlocks[0];
                    CimClasses.Element.ChildElements.Add(el);
                    CimClasses.ChildViewModels.Add(new ElementViewModel(el, CimClasses));
                    CimClasses.SetRemovableForChildren();
                }
            }
            HasUnsavedChanges = false;
        }
        public XmlDocument ExportToXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("Root");
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(FunctionDefinitions.Element, xmlDoc));
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(CimClasses.Element, xmlDoc));
            xmlDoc.AppendChild(rootElement);
            HasUnsavedChanges = false;
            return xmlDoc;
        }

        public void AddCimClass()
        {
            CimClasses.AddNewChildElement(ElementModelProvider.GetElementModelByName("CimClass"));
            foreach(var c in CimClasses.ChildViewModels)
            {
                c.OnPropertyChanged("IsMovableUp");
                c.OnPropertyChanged("IsMovableDown");
                c.OnPropertyChanged("IsMovable");
            }
        }
        public void AddFunctionDefinition(string name)
        {
            FunctionDefinitions.AddNewChildElement(ElementModelProvider.GetElementModelByName("FunctionDefinition"));
            FunctionDefinitions.ChildViewModels.Last().Attributes[0].Value = name;
            foreach (var c in FunctionDefinitions.ChildViewModels)
            {
                c.OnPropertyChanged("IsMovableUp");
                c.OnPropertyChanged("IsMovableDown");
                c.OnPropertyChanged("IsMovable");
            }
            ElementModelProvider.AddNewFunctionDefinition(name);
            foreach (var c in CimClasses.ChildViewModels)
                c.SetReplacable();
        }
        public void RenameFunction(string oldName, string newName)
        {
            FunctionDefinitions.ChildViewModels.First(x => x.Attributes[0].Value.Equals(oldName)).Attributes[0].Value = newName;
            ElementModelProvider.RenameFunction(oldName, newName);
            foreach (var c in CimClasses.ChildViewModels)
                c.RenameFunction(oldName, newName);
        }
        public void RemoveFunctionDefinition(ElementViewModel functionDefinition)
        {
            functionDefinition.DeleteElement();
            ElementModelProvider.RemoveFunctionDefinition(functionDefinition.Attributes[0].Value);
            foreach (var c in CimClasses.ChildViewModels)
                c.SetReplacable();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!propertyName.Equals("HasUnsavedChanges"))
                HasUnsavedChanges = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}