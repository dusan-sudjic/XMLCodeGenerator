using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.View;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static List<ProviderReaderClass> ProviderReaderClasses = new();
        public static List<SourceProviderEntity> SourceProviderEntities = new();
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
        public ICommand OpenNewProjectCommand { get; set; }
        public static DocumentViewModel Document { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ElementModelProvider.LoadModel();
            Document = new DocumentViewModel();
            Document.setup();
            AddClassCommand = new RelayCommand(ExecuteAddNewCimClassCommand);
            ExportToXmlCommand = new RelayCommand(ExecuteExportToXmlCommand);
            OpenExistingFileCommand = new RelayCommand(ExecuteOpenExistingFileCommand);
            OpenNewProjectCommand = new RelayCommand(ExecuteOpenNewProjectCommand);
            IsProviderReaderImported = false;
            xmlPreviewControl = (XmlPreviewUserControl)this.FindName("xmlPreview");
            this.DataContext = this;
        }

        public static void BindElementToXMLPreview(ElementViewModel element)
        {
            xmlPreviewControl.XmlElements = new List<XmlElement> { XmlElementFactory.GetXmlElement(element.Element) };
        }

        public void AddNewCimClass_Click(object sender, RoutedEventArgs e)
        {
            TabControl tab = (TabControl)FindName("TabControl");
            if (tab.SelectedIndex == 0)
            {
                Document.AddCimClass();
                ScrollToBottomSmoothlyAsync("CimClassesScroll");
            }
            else
                AddNewCimFunction();
        }
        private void AddNewCimFunction()
        {
            CreateNewFunctionWindow window = new CreateNewFunctionWindow();
            if (window.ShowDialog() == true)
            {
                if (ElementModelProvider.FunctionNameAlreadyInUse(window.Name))
                { 
                    MessageBox.Show("Function with name " + window.Name + " already exists.");
                    return;
                }
                else
                {
                    Document.AddFunctionDefinition(window.Name);
                    ScrollToBottomSmoothlyAsync("CimFunctionsScroll");
                }
            }
        }
        public static void RenameFunction(string oldFunctionName, string newFunctionName)
        {
            Document.RenameFunction(oldFunctionName, newFunctionName);
        }
        public static void RemoveCimClass(ElementViewModel cimClassViewModel)
        {
            cimClassViewModel.DeleteElement();
        }
        public static void RemoveFunctionDefinition(ElementViewModel functionDefinitionViewModel)
        {
            Document.RemoveFunctionDefinition(functionDefinitionViewModel);
        }
        public void ExecuteOpenNewProjectCommand(object parameter)
        {
            if (Document.HasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes in current project first?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveToXml();
                    if (Document.HasUnsavedChanges)
                        return;
                }
                else if (result != MessageBoxResult.No)
                    return;
            }
            Document.Reset();
        }

        public void ExecuteAddNewCimClassCommand(object parameter)
        {
            TabControl tab = (TabControl)FindName("TabControl");
            if (tab.SelectedIndex == 0)
                Document.AddCimClass();
            else
                AddNewCimFunction();
        }
        public void ExecuteOpenExistingFileCommand(object parameter)
        {
            if (Document.HasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes in current project first?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveToXml();
                    if (Document.HasUnsavedChanges)
                        return;
                }
                else if (result != MessageBoxResult.No)
                    return;
            }
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    Document.LoadFromXmlDocument(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading or processing XML file: {ex.Message}");
                }
            }
        }
        public void ExecuteExportToXmlCommand(object parameter)
        {
            SaveToXml();
        }

        private static void SaveToXml()
        {
            if (Document.OutputPath == null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                saveFileDialog.OverwritePrompt = false;
                if (saveFileDialog.ShowDialog() == true)
                    Document.OutputPath = saveFileDialog.FileName;
            }
            try
            {
                if (Document.OutputPath == null)
                    return;
                XmlDocument xmlDoc = Document.ExportToXmlDocument();
                xmlDoc.Save(Document.OutputPath);
                MessageBox.Show("XML saved successfully.");
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
        public void OpenNewProject(object sender, RoutedEventArgs e)
        {
            ExecuteOpenNewProjectCommand(null);
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
                        ProviderReaderClasses.Add(new ProviderReaderClass(type));
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
                            string name = personNode.SelectSingleNode("Name")?.InnerText.Trim();
                            SourceProviderEntity entity = new SourceProviderEntity(name);
                            foreach (XmlNode attributeNode in personNode.SelectNodes("EntityAttribute"))
                                entity.Attributes.Add(new SourceProviderAttribute(attributeNode.Attributes["Name"]?.Value, name));
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
}