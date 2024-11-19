using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml;

namespace XMLCodeGenerator.View
{
    public partial class XmlPreviewUserControl : UserControl, INotifyPropertyChanged
    {
        public List<XmlElement> _xmlElements { get; set; }    

        public XmlPreviewUserControl()
        {
            InitializeComponent();
            XmlElements = new List<XmlElement>();
            DataContext = this;
        }
        public List<XmlElement> XmlElements
        {
            get { return _xmlElements; }
            set
            {
                _xmlElements = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
