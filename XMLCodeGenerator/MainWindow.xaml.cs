﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using XMLCodeGenerator.Commands;
using XMLCodeGenerator.Model;
using XMLCodeGenerator.View;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<ClassInfo> ProviderReaderClasses = new();
        public List<EntityInfo> SourceProviderEntities = new();
        public static string OutputPath { get; private set; }
        private bool _isProviderReaderImported;
        public bool IsProviderReaderImported { 
            get => _isProviderReaderImported; 
            set { 
                if(value!=_isProviderReaderImported) 
                { 
                    _isProviderReaderImported = value;
                    OnPropertyChanged();
                }
            } 
        }
        private static bool _hasUnsavedChages;
        public static bool HasUnsavedChanges
        {
            get => _hasUnsavedChages;
            set
            {
                if (value != _hasUnsavedChages)
                {
                    _hasUnsavedChages = value;
                    UnsavedButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                    SavedButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
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
        public static XmlPreviewUserControl xmlPreviewControl { get; set; }
        public ICommand AddClassCommand { get; set; }
        public ICommand ExportToXmlCommand { get; set; }
        public ICommand OpenExistingFileCommand { get; set; }
        public static List<ElementViewModel> CimClasses { get; set; }
        public static List<ElementViewModel> FunctionDefinitions { get; set; }
        public static StackPanel CimClassesStackPanel { get; set; }
        public static StackPanel FunctionDefinitionsStackPanel { get; set; }
        private static TextBlock XMLPreviewTextBlock;
        private static Button UnsavedButton;
        private static Button SavedButton;
        public MainWindow()
        {
            InitializeComponent();
            CimClasses = new List<ElementViewModel>();
            FunctionDefinitions = new List<ElementViewModel>();
            AddClassCommand = new RelayCommand(ExecuteAddNewCimClassCommand);
            ExportToXmlCommand = new RelayCommand(ExecuteExportToXmlCommand);
            OpenExistingFileCommand = new RelayCommand(ExecuteOpenExistingFileCommand);
            this.DataContext = this;
            CimClassesStackPanel = (StackPanel)this.FindName("Stack");
            SavedButton = (Button)this.FindName("Saved");
            UnsavedButton = (Button)this.FindName("Unsaved");
            FunctionDefinitionsStackPanel = (StackPanel)this.FindName("FunctionsStack");
            XMLPreviewTextBlock = (TextBlock)this.FindName("XMLPreview");
            IsProviderReaderImported = false;
            HasUnsavedChanges = false;
            xmlPreviewControl = (XmlPreviewUserControl)this.FindName("xmlPreview");
            ModelProvider.LoadModel();
        }

        public static void BindElementToXMLPreview(ElementViewModel element)
        {
            xmlPreviewControl.XmlElements = new List<XmlElement> { XmlElementFactory.GetXmlElement(element.Element) };
        }
        //private static Inline[] ParseAndStylizeText(string text)
        //{
        //    var inlines = new List<Inline>();

        //    var regex = new Regex(@"\[b\](?<boldText>.*?)\[\/b\]|\[r\](?<redText>.*?)\[\/r\]|(?<normalText>[^[]+)");
        //    var matches = regex.Matches(text);

        //    foreach (Match match in matches)
        //    {
        //        if (match.Groups["boldText"].Success)
        //            inlines.Add(new Run(match.Groups["boldText"].Value) { FontWeight = FontWeights.Bold });
        //        else if (match.Groups["redText"].Success)
        //            inlines.Add(new Run(match.Groups["redText"].Value) { Foreground = Brushes.DarkOrange, FontWeight=FontWeights.DemiBold });
        //        else if (match.Groups["normalText"].Success)
        //            inlines.Add(new Run(match.Groups["normalText"].Value));
        //    }
        //    return inlines.ToArray();
        //}

        public void AddNewCimClass_Click(object sender, RoutedEventArgs e)
        {
            TabControl tab = (TabControl)FindName("TabControl");
            if (tab.SelectedIndex == 0)
                AddNewCimClass();
            else
                AddNewCimFunction();
        }

        private void AddNewCimClass()
        {
            ElementModel bp = ModelProvider.GetElementModel("CimClass");
            ElementViewModel element = new ElementViewModel(new Element(bp));
            CimClasses.Add(element);
            StackPanel stackPanel = (StackPanel)this.FindName("Stack");
            stackPanel.Children.Add(new ElementUserControl(element));
            ScrollToBottomSmoothlyAsync("CimClassesScroll");
        }
        private void AddNewCimFunction()
        {
            ElementModel bp = ModelProvider.GetElementModel("FunctionDefinition");
            ElementViewModel element = new ElementViewModel(new Element(bp));
            FunctionDefinitions.Add(element);
            StackPanel stackPanel = (StackPanel)this.FindName("FunctionsStack");
            stackPanel.Children.Add(new ElementUserControl(element));
            ScrollToBottomSmoothlyAsync("CimFunctionsScroll");
        }

        public static void RemoveCimClass(ElementUserControl uc)
        {
            ElementViewModel class1 = uc.Element as ElementViewModel;
            if (class1 == null)
                return;
            CimClasses.Remove(class1);
            CimClassesStackPanel.Children.Remove(uc);
        }
        public static void RemoveFunctionDefinition(ElementUserControl uc)
        {
            ElementViewModel function = uc.Element as ElementViewModel;
            if (function == null)
                return;
            FunctionDefinitions.Remove(function);
            FunctionDefinitionsStackPanel.Children.Remove(uc);
        }
        public void ExecuteAddNewCimClassCommand(object parameter)
        {
            TabControl tab = (TabControl)FindName("TabControl");
            if (tab.SelectedIndex == 0)
                AddNewCimClass();
            else
                AddNewCimFunction();
        }
        public void ExecuteOpenExistingFileCommand(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    OutputPath = filePath;
                    CimClasses = new();
                    StackPanel cimClassesStackPanel = (StackPanel)this.FindName("Stack");
                    cimClassesStackPanel.Children.Clear();
                    StackPanel functionDefinitionsStackPanel = (StackPanel)this.FindName("FunctionsStack");
                    functionDefinitionsStackPanel.Children.Clear();
                    XmlNodeList cimClassNodes = xmlDoc.SelectNodes("//CimClass");
                    if (cimClassNodes != null)
                    {
                        foreach (XmlElement cimClassNode in cimClassNodes)
                        {
                            ElementViewModel elemVM = new ElementViewModel(XmlElementFactory.GetElement(cimClassNode));
                            CimClasses.Add(elemVM);
                            cimClassesStackPanel.Children.Add(new ElementUserControl(elemVM));
                        }
                    }
                    FunctionDefinitions = new();
                    XmlNodeList functionDefinitionNodes = xmlDoc.SelectNodes("//FunctionDefinition");
                    if (functionDefinitionNodes != null)
                    {
                        foreach (XmlElement functionDefinitionNode in functionDefinitionNodes)
                        {
                            ElementViewModel elemVM = new ElementViewModel(XmlElementFactory.GetElement(functionDefinitionNode));
                            FunctionDefinitions.Add(elemVM);
                            functionDefinitionsStackPanel.Children.Add(new ElementUserControl(elemVM));
                        }
                    }
                    HasUnsavedChanges = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
                }
            }
        }
        public void ExecuteExportToXmlCommand(object parameter)
        {
            if (OutputPath == null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog.OverwritePrompt = false;
                if (saveFileDialog.ShowDialog() == true)
                {
                    OutputPath = saveFileDialog.FileName;
                }
            }
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("Root");
                var functionDefinitions = xmlDoc.CreateElement("FunctionDefinitions");
                foreach (var functionDefinition in FunctionDefinitions)
                    functionDefinitions.AppendChild(XmlElementFactory.GetXmlElement(functionDefinition.Element,xmlDoc));
                rootElement.AppendChild(functionDefinitions);
                xmlDoc.AppendChild(rootElement);
                var cimClasses = xmlDoc.CreateElement("CimClasses");
                foreach (var cimClass in CimClasses)
                    cimClasses.AppendChild(XmlElementFactory.GetXmlElement(cimClass.Element,xmlDoc));
                rootElement.AppendChild(cimClasses);

                xmlDoc.Save(OutputPath);

                MessageBox.Show("XML saved successfully.");
                HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XML file: {ex.Message}");
            }
        }
        public void ExportToXml(object sender, RoutedEventArgs e)
        {
            ExecuteExportToXmlCommand(null);
        }
        

        public void ImportProviderReader(object sender, RoutedEventArgs e)
        {
            ProviderReaderClasses.Clear();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DLL Files (*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(filePath);
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        string className = type.FullName;
                        className += type.BaseType.Name!="Object" ? $" (inherits from {type.BaseType.Name})":"";
                        ProviderReaderClasses.Add(new ClassInfo { ClassName = className, Properties = GetClassProperties(type) });
                    }
                    IsProviderReaderImported = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading classes from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ImportSourceProvider(object sender, RoutedEventArgs e)
        {
            SourceProviderEntities.Clear();
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    XmlNodeList entitityNodes = xmlDoc.SelectNodes("//Entity");

                    if (entitityNodes != null)
                    {
                        foreach (XmlNode personNode in entitityNodes)
                        {
                            string name = personNode.SelectSingleNode("Name")?.InnerText;
                            EntityInfo entity = new EntityInfo { Name = name.Trim(), Attributes = new() };
                            foreach (XmlNode attributeNode in personNode.SelectNodes("EntityAttribute"))
                                entity.Attributes.Add(attributeNode.Attributes["Name"]?.Value);
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
        }

        public void OpenExistingFile(object sender, RoutedEventArgs e)
        {
            ExecuteOpenExistingFileCommand(null);
        }
        private List<string> GetClassProperties(Type type)
        {
            List<string> properties = new List<string>();
            foreach (var property in type.GetProperties())
                properties.Add($".\t{property.Name}: {property.PropertyType.Name}\n");
            return properties;
        }
        public void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var border = (Border)thumb.Parent;

            double deltaX = e.HorizontalChange;
            double deltaY = e.VerticalChange;

            border.Width = Math.Max(border.Width + deltaX, thumb.DesiredSize.Width);
            border.Height = Math.Max(border.Height + deltaY, thumb.DesiredSize.Height);
        }
        private async void ScrollToBottomSmoothlyAsync(string name)
        {
            ScrollViewer scrollViewer = (ScrollViewer)this.FindName(name);
            double scrollHeight = scrollViewer.ScrollableHeight;
            double currentOffset = scrollViewer.VerticalOffset;
            double targetOffset = scrollHeight;
            int steps = 20;
            double stepSize = (targetOffset - currentOffset) / steps;
            for (int i = 0; i < steps; i++)
            {
                currentOffset += stepSize;
                scrollViewer.ScrollToVerticalOffset(currentOffset);
                await Task.Delay(10); 
            }
            scrollViewer.ScrollToBottom();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaximizeToWorkingArea();
        }

        private void MaximizeToWorkingArea()
        {
            var workingArea = SystemParameters.WorkArea;

            this.WindowState = WindowState.Normal; 
            this.WindowStyle = WindowStyle.SingleBorderWindow; 
            this.ResizeMode = ResizeMode.CanResize; 

            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;

            this.Topmost = true; 
            this.Topmost = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public record ClassInfo
    {
        public string ClassName { get; set; }
        public List<string> Properties { get; set; }
    }
    public record EntityInfo
    {
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
    }
}