using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;
using Xceed.Wpf.AvalonDock.Controls;
using XMLCodeGenerator.Behaviors;
using XMLCodeGenerator.Commands;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.View;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static ProvidersViewModel ProvidersViewModel { get; set; } = new();
        public static XmlPreviewUserControl xmlPreviewControl { get; set; }
        public ICommand AddClassCommand { get; set; }
        public ICommand ExportToXmlCommand { get; set; }
        public ICommand OpenExistingFileCommand { get; set; }
        public ICommand OpenNewProjectCommand { get; set; }
        public static DocumentViewModel Document { get; set; } = new();
        public static ItemsControl ItemsControlClasses { get; set; }
        public static ItemsControl ItemsControlFunctions { get; set; }
        public static ItemsControl ItemsControlPreprocess { get; set; }
        public static ItemsControl ItemsControlRewriting { get; set; }
        public static ScrollViewer CimClassesScrollViewer { get; set; }
        public static ScrollViewer CimFunctionsScrollViewer { get; set; }
        public static ScrollViewer PreprocessScrollViewer { get; set; }
        public static ScrollViewer RewritingScrollViewer { get; set; }
        public static int SelectedTabIndex { get; set; } = -1;
        public MainWindow()
        {
            InitializeComponent();
            Document.Setup();
            AddClassCommand = new RelayCommand(ExecuteAddNewCimClassCommand);
            ExportToXmlCommand = new RelayCommand(ExecuteExportToXmlCommand);
            OpenExistingFileCommand = new RelayCommand(ExecuteOpenExistingFileCommand);
            OpenNewProjectCommand = new RelayCommand(ExecuteOpenNewProjectCommand);
            xmlPreviewControl = (XmlPreviewUserControl)this.FindName("xmlPreview");
            ItemsControlClasses = itemsControlClasses;
            ItemsControlFunctions = itemsControlFunctions;
            ItemsControlPreprocess = itemsControlPreprocess;
            ItemsControlRewriting = itemsControlRewriting;
            CimClassesScrollViewer = CimClassesScroll;
            CimFunctionsScrollViewer = CimFunctionsScroll;
            PreprocessScrollViewer = PreprocessScroll;
            RewritingScrollViewer = RewritingScroll;
            DataContext = this;
        }

        public void ExportToXml(object sender, RoutedEventArgs e)
        {
            ExecuteExportToXmlCommand(null);
        }
        public void OpenNewProject(object sender, RoutedEventArgs e)
        {
            ExecuteOpenNewProjectCommand(null);
        }
        public void AddNewCimClass_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAddNewCimClassCommand(null);
        }
        public void OpenExistingFile(object sender, RoutedEventArgs e)
        {
            ExecuteOpenExistingFileCommand(null);
        }
        public static void BindElementToXMLPreview(ElementViewModel element)
        {
            xmlPreviewControl.XmlElements = new List<XmlElement> { XmlElementFactory.GetXmlElement(element.Element) };
        }
        public static void UpdateFunctionCallsCounter(string functionName)
        {
            Document.UpdateFunctionCallsCounte(functionName);
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
                }
            }
        }
        public static string GetInterfaceBasedOnTabForElement(DependencyObject elementUC)
        {
            while (elementUC != null)
            {
                if (elementUC is TabControl tabItem)
                {
                    switch (tabItem.SelectedIndex)
                    {
                        case 2: return "IMappingPreProcessorProcedure";
                        case 3: return "IMappingRewritingProcedure";
                        default: return "IOperator";
                    }
                }
                elementUC = VisualTreeHelper.GetParent(elementUC);
            }
            return null;
        }
        public static void RenameFunction(string oldFunctionName, string newFunctionName)
        {
            Document.RenameFunction(oldFunctionName, newFunctionName);
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
                    ExecuteExportToXmlCommand(null);
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
            else if (tab.SelectedIndex == 1)
                AddNewCimFunction();
            else if (tab.SelectedIndex == 2)
                Document.AddPreprocessProcedure();
            else if (tab.SelectedIndex == 3)
                Document.AddRewritingProcedure();
        }
        public void ExecuteOpenExistingFileCommand(object parameter)
        {
            if (Document.HasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes in current project first?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ExecuteExportToXmlCommand(null);
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
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        public void ExecuteExportToXmlCommand(object parameter)
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
        public void ImportCimProfile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DLL Files (*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                ProvidersViewModel.CimProfilePath = openFileDialog.FileName;
                LoadCimProfile();
            }
        }
        public static void LoadCimProfile()
        {
            ProvidersViewModel.LoadCimProfile();
        }
        public void ImportSourceProvider(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                ProvidersViewModel.SourceProviderPath = openFileDialog.FileName;
                LoadSourceProvider();
            }
        }
        public static void LoadSourceProvider()
        {
            ProvidersViewModel.LoadSourceProvider();
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
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTabIndex = TabControl.SelectedIndex;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}