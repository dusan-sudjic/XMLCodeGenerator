using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class MappingClassesWindow : Window, INotifyPropertyChanged
    {
        const string FILE_NOT_IMPORTED_MESSAGE = "File not imported.";
        public static string FilePath { get; set; } = FILE_NOT_IMPORTED_MESSAGE;
        public string MappingInterface { get; set; }
        public ElementViewModel Element { get; set; }
        public ClassItem SelectedClass { get; set; }
        public ObservableCollection<ClassItem> Classes { get; set; } = new();
        public MappingClassesWindow(ElementViewModel element, string mappingInterface)
        {
            InitializeComponent();
            DataContext = this;
            Element = element;
            MappingInterface = mappingInterface;
            if (!FilePath.Equals(FILE_NOT_IMPORTED_MESSAGE))
                LoadClassItemsFromFile();
            listBox.SelectedIndex = 0;
            search.Focus();
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
                    if (type.GetInterface(MappingInterface) != null)
                        Classes.Add(new ClassItem
                        {
                            AssemblyName = type.Namespace,
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
            Select();
        }

        private void Select()
        {
            foreach (var attr in Element.Attributes)
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                OnDownArrowPressed();
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                OnUpArrowPressed();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                Select();
                e.Handled = true;
            }
        }

        private void OnDownArrowPressed()
        {
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = 0;
                return;
            }
            if (listBox.SelectedIndex == listBox.Items.Count - 1)
            {
                listBox.SelectedIndex = 0;
                return;
            }
            listBox.SelectedIndex++;
        }
        private void OnUpArrowPressed()
        {
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                return;
            }
            if (listBox.SelectedIndex == 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                return;
            }
            listBox.SelectedIndex--;
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ClassItem> newList = new();
            foreach (var s in Classes)
            {
                if (s.ClassName.ToLower().Contains(search.Text.ToLower()))
                    newList.Add(s);
            }
            listBox.ItemsSource = newList;
            if (listBox.SelectedIndex == -1) listBox.SelectedIndex = 0;
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
