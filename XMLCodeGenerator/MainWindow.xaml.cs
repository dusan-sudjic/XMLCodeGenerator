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
        private static TabControl RightTabControl { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Document.Setup();
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
            RightTabControl = rightTabControl;
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
            ElementUserControl.AddChildElement(Document.CimClasses);
        }
        public void AddNewCimFunction_Click(object sender, RoutedEventArgs e)
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
        public void AddNewPreprocessProcedure_Click(object sender, RoutedEventArgs e)
        {
            ElementUserControl.AddChildElement(Document.PreProcessProcedures);
        }
        public void AddNewRewritingProcedure_Click(object sender, RoutedEventArgs e)
        {
            ElementUserControl.AddChildElement(Document.RewritingProcedures);
        }
        public void OpenExistingFile(object sender, RoutedEventArgs e)
        {
            ExecuteOpenExistingFileCommand(null);
        }
        public static void BindElementToXMLPreview(ElementViewModel element)
        {
            RightTabControl.SelectedIndex = 0;
            xmlPreviewControl.XmlElements = new List<XmlElement> { XmlElementFactory.GetXmlElement(element.Element) };
        }
        public static void UpdateFunctionCallsCounter(string functionName)
        {
            Document.UpdateFunctionCallsCounte(functionName);
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
        public static async void ScrollToElement(ElementViewModel elementVM, bool isNewElement = true)
        {
            await Task.Delay(100);
            Scroll(elementVM, GetItemsControl());
            if (isNewElement)
            {
                elementVM.IsNew = true;
                await Task.Delay(1300);
                elementVM.IsNew = false;
            }
        }
        private static ItemsControl GetItemsControl()
        {
            ItemsControl itemsControl = MainWindow.ItemsControlClasses;
            switch (MainWindow.Document.CurrentlyDisplayedTab)
            {
                case 1: { itemsControl = MainWindow.ItemsControlFunctions; break; }
                case 2: { itemsControl = MainWindow.ItemsControlPreprocess; break; }
                case 3: { itemsControl = MainWindow.ItemsControlRewriting; break; }
            }
            return itemsControl;
        }
        private static void Scroll(ElementViewModel element, ItemsControl parent)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                var container = parent.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                if (container != null)
                {
                    var elementUserControl = FindVisualChild<ElementUserControl>(container);
                    if (elementUserControl != null)
                    {
                        if (elementUserControl.Element == element)
                        {
                            MainWindow.ScrollToElementUserControl(elementUserControl);
                            return;
                        }
                        Scroll(element, elementUserControl.itemsControlChildren);
                    }
                }
            }
        }
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T childOfType)
                {
                    return childOfType;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
        public static void ScrollToElementUserControl(FrameworkElement targetElement)
        {
            ScrollViewer scrollViewer = null;
            switch (Document.CurrentlyDisplayedTab)
            {
                case 0: { scrollViewer = MainWindow.CimClassesScrollViewer; break; }
                case 1: { scrollViewer = MainWindow.CimFunctionsScrollViewer; break; }
                case 2: { scrollViewer = MainWindow.PreprocessScrollViewer; break; }
                case 3: { scrollViewer = MainWindow.RewritingScrollViewer; break; }
            }
            GeneralTransform transform = targetElement.TransformToAncestor(scrollViewer);
            Point targetPosition = transform.Transform(new Point(0, 0));
            double spaceAbove = scrollViewer.ActualHeight / 5;
            double targetOffset = targetPosition.Y + scrollViewer.VerticalOffset - spaceAbove;
            DoubleAnimation verticalAnimation = new DoubleAnimation
            {
                From = scrollViewer.VerticalOffset,
                To = targetOffset,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            storyboard.Begin();
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Document.CurrentlyDisplayedTab = TabControl.SelectedIndex;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}