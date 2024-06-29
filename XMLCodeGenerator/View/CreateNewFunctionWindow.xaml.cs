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
        public CreateNewFunctionWindow()
        {
            InitializeComponent();
            DataContext = this;
            textBox.Focus();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            if(textBox.Text.Length > 0)
            {
                Name = textBox.Text;
                this.DialogResult = true;
            }
        }
    }
}
