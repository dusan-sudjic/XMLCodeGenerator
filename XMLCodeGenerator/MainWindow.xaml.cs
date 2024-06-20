using System.Collections.Specialized;
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
using XMLCodeGenerator.Commands;
using XMLCodeGenerator.Model;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;
using XMLCodeGenerator.View;
using static System.Net.Mime.MediaTypeNames;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<ClassInfo> ProviderReaderClasses = new();
        public List<EntityInfo> SourceProviderEntities = new();
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
        public ICommand AddClassCommand { get; set; }
        public static List<IElement> CimClasses { get; set; }
        public static StackPanel CimClassesStackPanel { get; set; }
        private static TextBlock XMLPreviewTextBlock;
        public MainWindow()
        {
            InitializeComponent();
            CimClasses = new List<IElement>();
            AddClassCommand = new RelayCommand(ExecuteAddNewCimClassCommand);
            this.DataContext = this;
            CimClassesStackPanel = (StackPanel)this.FindName("Stack");
            XMLPreviewTextBlock = (TextBlock)this.FindName("XMLPreview");
            IsProviderReaderImported = false;
            BlueprintsProvider.LoadModel();
            IElement element = ElementFactory.CreateElementFromBlueprint(BlueprintsProvider.GetBlueprint("Addition"));
            IElement element2 = element.ChildElements[1];
            List<ElementBlueprint> bp = BlueprintsProvider.GetReplacementBlueprintsForElement(element2);
            bp.Clear();
        }

        public static void BindElementToXMLPreview(IElement element)
        {
            XMLPreviewTextBlock.Inlines.Clear();
            XMLPreviewTextBlock.Inlines.AddRange(ParseAndStylizeText(XMLElementConverter.ConvertElementToXML(element)));
        }
        private static Inline[] ParseAndStylizeText(string text)
        {
            var inlines = new List<Inline>();

            var regex = new Regex(@"\[b\](?<boldText>.*?)\[\/b\]|\[r\](?<redText>.*?)\[\/r\]|(?<normalText>[^[]+)");
            var matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                if (match.Groups["boldText"].Success)
                    inlines.Add(new Run(match.Groups["boldText"].Value) { FontWeight = FontWeights.Bold });
                else if (match.Groups["redText"].Success)
                    inlines.Add(new Run(match.Groups["redText"].Value) { Foreground = Brushes.DarkOrange, FontWeight=FontWeights.DemiBold });
                else if (match.Groups["normalText"].Success)
                    inlines.Add(new Run(match.Groups["normalText"].Value));
            }
            return inlines.ToArray();
        }

        public void AddNewCimClass_Click(object sender, RoutedEventArgs e)
        {
            AddNewCimClass();
        }

        private void AddNewCimClass()
        {
            ElementBlueprint bp = BlueprintsProvider.GetBlueprint("CimClass");
            IElement element = ElementFactory.CreateElementFromBlueprint(bp);
            CimClasses.Add(element);
            StackPanel stackPanel = (StackPanel)this.FindName("Stack");
            stackPanel.Children.Add(new ElementUserControl(element));
            ScrollToBottomSmoothlyAsync();
        }

        public static void RemoveCimClass(ElementUserControl uc)
        {
            CimClass class1 = uc.Element as CimClass;
            if (class1 == null)
                return;
            CimClasses.Remove(class1);
            CimClassesStackPanel.Children.Remove(uc);
        }
        public void ExecuteAddNewCimClassCommand(object parameter)
        {
            AddNewCimClass();
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
        private async void ScrollToBottomSmoothlyAsync()
        {
            ScrollViewer scrollViewer = (ScrollViewer)this.FindName("CimClassesScroll");
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