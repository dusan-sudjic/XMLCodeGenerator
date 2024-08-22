using System;
using System.Collections.Generic;
using System.Linq;
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

namespace XMLCodeGenerator.View
{
    public partial class CreateNewFunctionWindow : Window
    {
        public string Name {  get; set; }
        private string oldName;
        public CreateNewFunctionWindow(string oldName = "")
        {
            InitializeComponent();
            DataContext = this;
            this.oldName = oldName;
            if(this.oldName.Length>0)
            {
                this.Title = "Rename function";
                label.Content = "Enter a new name for function " + this.oldName;
            }
            Name = oldName;
            textBox.Text = oldName;
            textBox.Focus();
        }
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SubmitClick(sender, e);
        }
        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            if(textBox.Text.Length > 0 && !textBox.Text.Contains(" "))
            {
                Name = textBox.Text;
                if (Name.Equals(oldName))
                {
                    MessageBox.Show("New name for the function can't be the same as before.");
                    return;
                }
                this.DialogResult = true;
            }
        }
    }
}
