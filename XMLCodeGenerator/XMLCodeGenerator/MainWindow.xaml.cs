using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XMLCodeGenerator
{
    public partial class MainWindow : Window
    {
        public List<ClassInfo> Classes = new();
        public MainWindow()
        {
            InitializeComponent();
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
                    classListBox.ItemsSource = Classes;
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
    }

    public record ClassInfo
    {
        public string ClassName { get; set; }
        public string Properties { get; set; }
    }
}