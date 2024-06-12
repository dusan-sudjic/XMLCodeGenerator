using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
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
using XMLCodeGenerator.Commands;
using XMLCodeGenerator.Model;
using XMLCodeGenerator.Model.BuildingBlocks;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.Elements.BooleanOperators;
using XMLCodeGenerator.Model.Elements.Conditions;
using XMLCodeGenerator.Model.Elements.GetOperators;
using XMLCodeGenerator.Model.Elements.MathOperators;
using XMLCodeGenerator.View;
using static System.Net.Mime.MediaTypeNames;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window
    {
        public List<ClassInfo> Classes = new();
        public ICommand AddClassCommand { get; set; }
        public static List<ICim> CimClasses { get; set; }
        public static StackPanel CimClassesStackPanel { get; set; }
        private static TextBlock XMLPreviewTextBlock;
        public MainWindow()
        {
            InitializeComponent();
            CimClasses = new List<ICim>();
            AddClassCommand = new RelayCommand(ExecuteAddNewCimClassCommand);
            var class1 = new CimClassElement();
            class1.Attributes.Where(a => a.Name == "name").ToList()[0].Value = "ACLineSegment";
            var property1 = new CimPropertyElement();
            property1.Attributes.Where(a => a.Name == "name").ToList()[0].Value = "ConnectedCircuit";
            var property2 = new CimPropertyElement();
            property2.Attributes.Where(a => a.Name == "name").ToList()[0].Value = "SequenceNumber";
            var property3 = new CimPropertyElement();
            property3.Attributes.Where(a => a.Name == "name").ToList()[0].Value = "Circuit";
            class1.AddChildElementToContent(property1);
            //RootElement.AddChildElementToContent(new CimPropertyElement());
            class1.AddChildElementToContent(property2);
            //RootElement.AddChildElementToContent(new CimPropertyElement());
            class1.AddChildElementToContent(property3);
            var if1 = new IfElement();
            property1.AddChildElementToContent(if1);
            var cond1 = new ConditionElement();
            var less1 = new NumericComparator("LessThan");
            cond1.AddChildElementToContent(less1);
            if1.AddChildElementToContent(cond1);
            var getVal1 = new GetValueElement();
            var getVal12 = new GetValueElement();
            less1.AddChildElementToContent(getVal1);
            less1.AddChildElementToContent(getVal12);
            var abs = new UnaryMathOperation("AbsoluteValue");
            abs.AddChildElementToContent(new ConstantElement());
            if1.AddChildElementToContent(abs);
            property2.AddChildElementToContent(new GetValueElement());
            property3.AddChildElementToContent(new ConstantElement());
            //List<string> classes = ElementProvider.GetSupportedChildElements(RootElement);  //getting options
            //var instance = ElementProvider.CreateNewElement(classes[5]);  //choosing an option
            //RootElement.AddChildElementToContent(instance);
            //List<string> classes2 = ElementProvider.GetSupportedChildElements(instance);
            //var instance2 = ElementProvider.CreateNewElement(classes2[2]);
            //instance.AddChildElementToContent(instance2);
            //instance2.AddChildElementToContent(ElementProvider.CreateNewElement(classes2[0]));
            ////instance2.AddChildElementToContent(ElementProvider.CreateNewElement(classes2[0]));
            //instance.AddChildElementToContent(ElementProvider.CreateNewElement(classes2[0]));
            this.DataContext = this;

            CimClassesStackPanel = (StackPanel)this.FindName("Stack");
            XMLPreviewTextBlock = (TextBlock)this.FindName("XMLPreview");
            CimClasses.Add(class1);
            CimClassesStackPanel.Children.Add(new ElementUserControl(class1));
            CimClasses.Add(class1);
            CimClassesStackPanel.Children.Add(new ElementUserControl(class1));
            CimClasses.Add(class1);
            CimClassesStackPanel.Children.Add(new ElementUserControl(class1));
            CimClasses.Add(class1);
            CimClassesStackPanel.Children.Add(new ElementUserControl(class1));
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
                {
                    inlines.Add(new Run(match.Groups["boldText"].Value) { FontWeight = FontWeights.Bold });
                }
                else if (match.Groups["redText"].Success)
                {
                    inlines.Add(new Run(match.Groups["redText"].Value) { Foreground = Brushes.DarkOrange, FontWeight=FontWeights.DemiBold });
                }
                else if (match.Groups["normalText"].Success)
                {
                    inlines.Add(new Run(match.Groups["normalText"].Value));
                }
            }
            return inlines.ToArray();
        }

        public void AddNewCimClass_Click(object sender, RoutedEventArgs e)
        {
            AddNewCimClass();
        }

        private void AddNewCimClass()
        {
            CimClassElement class1 = new CimClassElement();
            CimClasses.Add(class1);
            StackPanel stackPanel = (StackPanel)this.FindName("Stack");
            stackPanel.Children.Add(new ElementUserControl(class1));
            ScrollToBottomSmoothlyAsync();
        }

        public static void RemoveCimClass(ElementUserControl uc)
        {
            CimClassElement class1 = uc.Element as CimClassElement;
            if (class1 == null)
                return;
            CimClasses.Remove(class1);
            CimClassesStackPanel.Children.Remove(uc);
        }
        public void ExecuteAddNewCimClassCommand(object parameter)
        {
            AddNewCimClass();
        }
        public void ReadClassesFromFileClick(object sender, RoutedEventArgs e)
        {
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
                        string properties = string.Join("", GetClassProperties(type));
                        Classes.Add(new ClassInfo { ClassName = className, Properties = properties });
                    }
                    //classListBox.ItemsSource = Classes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading classes from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //var workingArea = SystemParameters.WorkArea;

            //this.Width = workingArea.Width;
            //this.Height = workingArea.Height;

            //this.Left = workingArea.Left;
            //this.Top = workingArea.Top;
        }

        private void MaximizeToWorkingArea()
        {
            var workingArea = SystemParameters.WorkArea;

            this.WindowState = WindowState.Normal; // Ensure the window is in normal state first
            this.WindowStyle = WindowStyle.SingleBorderWindow; // Ensure the window style is correct
            this.ResizeMode = ResizeMode.CanResize; // Allow the window to be resized, to retain the border

            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;

            // Ensure the window is brought to the foreground
            this.Topmost = true; // Optional: keep the window on top of other windows
            this.Topmost = false; // Reset Topmost to allow interaction with other windows
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

    }

    public record ClassInfo
    {
        public string ClassName { get; set; }
        public string Properties { get; set; }
    }
}