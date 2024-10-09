using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.View;

namespace XMLCodeGenerator.ViewModel
{
    public sealed class DocumentViewModel : INotifyPropertyChanged
    {
        public ElementViewModel CimClasses { get; set; }
        public ElementViewModel FunctionDefinitions { get; set; }
        public ElementViewModel PreProcessProcedures { get; set; }
        public ElementViewModel RewritingProcedures { get; set; }
        public int CurrentlyDisplayedTab { get; set; }
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
        public SearchDocumentViewModel SearchDocumentViewModel { get; set; }
        public DocumentViewModel() 
        {
            ElementModelProvider.LoadModel();
            SearchDocumentViewModel = new SearchDocumentViewModel();
        }
        public void Setup()
        {
            CimClasses = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("CimClasses")));
            FunctionDefinitions = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("FunctionDefinitions")));
            PreProcessProcedures = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("PreProcessProcedures")));
            RewritingProcedures = new ElementViewModel(new Element(ElementModelProvider.GetElementModelByName("RewritingProcedures")));
            HasUnsavedChanges = false;
        }
        public void Reset()
        {
            CimClasses.Element.ChildElements.Clear();
            CimClasses.ChildViewModels.Clear();
            FunctionDefinitions.ChildViewModels.Clear();
            FunctionDefinitions.Element.ChildElements.Clear();
            PreProcessProcedures.ChildViewModels.Clear();
            PreProcessProcedures.Element.ChildElements.Clear();
            RewritingProcedures.ChildViewModels.Clear();
            RewritingProcedures.Element.ChildElements.Clear();
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
            XmlNodeList preprocessProcedures = document.SelectNodes("//PreProcessProcedures/PreProcessProcedure");
            if (preprocessProcedures != null)
            {
                foreach (XmlElement procedure in preprocessProcedures)
                {
                    Element el = XmlElementFactory.GetElement(procedure);
                    el.ParentContentBlock = PreProcessProcedures.Element.Model.ContentBlocks[0];
                    PreProcessProcedures.Element.ChildElements.Add(el);
                    PreProcessProcedures.ChildViewModels.Add(new ElementViewModel(el, PreProcessProcedures));
                    PreProcessProcedures.SetRemovableForChildren();
                }
            }
            XmlNodeList rewritingProcedures = document.SelectNodes("//RewritingProcedures/RewritingProcedure");
            if (rewritingProcedures != null)
            {
                foreach (XmlElement procedure in rewritingProcedures)
                {
                    Element el = XmlElementFactory.GetElement(procedure);
                    el.ParentContentBlock = RewritingProcedures.Element.Model.ContentBlocks[0];
                    RewritingProcedures.Element.ChildElements.Add(el);
                    RewritingProcedures.ChildViewModels.Add(new ElementViewModel(el, RewritingProcedures));
                    RewritingProcedures.SetRemovableForChildren();
                }
            }
            HasUnsavedChanges = false;
        }
        public XmlDocument ExportToXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("ElementMapping");
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(PreProcessProcedures.Element, xmlDoc));
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(RewritingProcedures.Element, xmlDoc));
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(FunctionDefinitions.Element, xmlDoc));
            rootElement.AppendChild(XmlElementFactory.GetXmlElement(CimClasses.Element, xmlDoc));
            xmlDoc.AppendChild(rootElement);
            foreach (var ns in XmlElementFactory.Namespaces.Keys)
                xmlDoc.DocumentElement.SetAttribute("xmlns:" + ns, XmlElementFactory.Namespaces[ns]);
            HasUnsavedChanges = false;
            return xmlDoc;
        }

        public void AddCimClass()
        {
            ElementViewModel newElement = CimClasses.AddNewChildElement(ElementModelProvider.GetElementModelByName("CimClass"));
            UpdateMovable(CimClasses);
            MainWindow.ScrollToElement(newElement);
        }
        public void UpdateFunctionCallsCounte(string functionName)
        {
            ElementViewModel vm = FunctionDefinitions.ChildViewModels.FirstOrDefault(c => c.Attributes[0].Value.Equals(functionName));
            vm.OnPropertyChanged("FunctionCalls");
        }
        public void AddPreprocessProcedure()
        {
            ElementViewModel newElement = PreProcessProcedures.AddNewChildElement(ElementModelProvider.GetElementModelByName("PreProcessProcedure"));
            UpdateMovable(PreProcessProcedures);
            MainWindow.ScrollToElement(newElement);
        }
        public void AddRewritingProcedure()
        {
            ElementViewModel newElement = RewritingProcedures.AddNewChildElement(ElementModelProvider.GetElementModelByName("RewritingProcedure"));
            UpdateMovable(RewritingProcedures);
            MainWindow.ScrollToElement(newElement);
        }
        public void AddFunctionDefinition(string name)
        {
            var newElement = FunctionDefinitions.AddNewChildElement(ElementModelProvider.GetElementModelByName("FunctionDefinition"));
            FunctionDefinitions.ChildViewModels.Last().Attributes[0].Value = name;
            UpdateMovable(FunctionDefinitions);
            ElementModelProvider.AddNewFunctionDefinition(name);
            foreach (var c in CimClasses.ChildViewModels)
                c.SetReplacable();
            MainWindow.ScrollToElement(newElement);
        }
        private void UpdateMovable(ElementViewModel vm)
        {
            foreach (var c in vm.ChildViewModels)
            {
                c.OnPropertyChanged("IsMovableUp");
                c.OnPropertyChanged("IsMovableDown");
                c.OnPropertyChanged("IsMovable");
            }
        }
        public void RenameFunction(string oldName, string newName)
        {
            FunctionDefinitions.ChildViewModels.First(x => x.Attributes[0].Value.Equals(oldName)).Attributes[0].Value = newName;
            ElementModelProvider.RenameFunction(oldName, newName);
            foreach (var c in CimClasses.ChildViewModels)
                c.RenameFunction(oldName, newName);
            foreach(var f in FunctionDefinitions.ChildViewModels)
                f.RenameFunction(oldName, newName);
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}