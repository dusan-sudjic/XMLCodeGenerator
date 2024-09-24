using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XMLCodeGenerator.Model.ProvidersConfig;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class MappingClassesWindow : Window, INotifyPropertyChanged
    {
        const string FILE_NOT_IMPORTED_MESSAGE = "File not imported.";
        public static string FilePath { get; set; } = FILE_NOT_IMPORTED_MESSAGE;
        public ElementViewModel Element { get; set; }
        public ClassItem SelectedClass { get; set; }
        public ObservableCollection<ClassItem> Classes { get; set; } = new();
        public MappingClassesWindow(ElementViewModel element)
        {
            InitializeComponent();
            DataContext = this;
            Element = element;
            if (!FilePath.Equals(FILE_NOT_IMPORTED_MESSAGE))
                LoadClassItemsFromFile();
        }
        
        private void ChooseDllFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "DLL Files (*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                PathLabel.Content = FilePath;
                LoadClassItemsFromFile();
            }
        }
        private void LoadClassItemsFromFile()
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(FilePath);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                    if (type.GetInterface(Element.Element.Model.MappingInterface) != null)
                        Classes.Add(new ClassItem
                        {
                            AssemblyName = assembly.FullName,
                            ClassName = type.Name,
                            FolderStructure = type.FullName
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var attr in Element.Attributes)
            {
                switch (attr.ValueMappingComponent)
                {
                    case Model.Elements.ValueMappingComponent.CLASS:
                        {
                            attr.Value = SelectedClass.ClassName;
                            break;
                        }
                    case Model.Elements.ValueMappingComponent.ASSEMBLY:
                        {
                            attr.Value = SelectedClass.AssemblyName;
                            break;
                        }
                    case Model.Elements.ValueMappingComponent.FOLDER_STRUCTURE:
                        {
                            attr.Value = SelectedClass.FolderStructure;
                            break;
                        }
                    default: break;
                }
            }
            this.DialogResult = true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class ClassItem
        {
            public string ClassName;
            public string AssemblyName;
            public string FolderStructure;
            public override string ToString()
            {
                return ClassName;
            }
        }
    }
}
