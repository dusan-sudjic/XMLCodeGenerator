using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XMLCodeGenerator.Behaviors;
using XMLCodeGenerator.Model;
using XMLCodeGenerator.Model.BuildingBlocks;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.Elements.GetOperators;
using XMLCodeGenerator.View.Attributes;

namespace XMLCodeGenerator.View
{
    public partial class ElementUserControl : UserControl, INotifyPropertyChanged
    {
        private bool _isMainLabelWithinViewport;
        public bool IsMainLabelWithinViewport
        {
            get { return _isMainLabelWithinViewport; }
            set
            {
                if (_isMainLabelWithinViewport != value)
                {
                    _isMainLabelWithinViewport = value;
                    OnPropertyChanged();
                }
            }
        }
        public StackPanel _childrenContainer;
        private StackPanel _attributesContainer;
        private StackPanel _unexpandableAttributesContainer;
        private Point _dragStartPoint;
        private UIElement _draggedElement;
        public string XML_Name { get { return Element.XML_Name; } set { } }
        public string AdditionalInfo { 
            get {
                if (Element is CimClassElement || Element is CimPropertyElement)
                    return "["+Element.Attributes.Where(a => a.Name == "name").ToList()[0].Value+"]";
                else
                    return "";
            } 
            set { } 
        }
        private int _originalIndex;
        public IElement Element { get; set; }
        public bool Opened = true;
        public ElementUserControl()
        {
            InitializeComponent();
        }
        public ElementUserControl(IElement element): this()
        {
            Element = element;
            DataContext = this;
            _childrenContainer = (StackPanel)this.FindName("ChildrenContainer");
            _attributesContainer = (StackPanel)this.FindName("AttributesStackPanel");
            _unexpandableAttributesContainer = (StackPanel)this.FindName("AttributesContainer");
            //StickyTextBlockBehavior.SetIsTopVisible(this, true);

            SetUpUI();
            SetButtons();
            if (Element.MaxContentSize == 0)
            {
                Border dock = (Border)this.FindName("Border");
                _childrenContainer.Visibility = Visibility.Collapsed;
                StackPanel SidePanel = (StackPanel)this.FindName("SidePanel");
                SidePanel.Visibility = Visibility.Collapsed;
                Label endLabel = (Label)this.FindName("EndLabel");
                endLabel.Visibility = Visibility.Collapsed;
                //dock.HorizontalAlignment = HorizontalAlignment.Left;
                Opened = false;
            }
        }

        public void SetButtons()
        {
            _childrenContainer.Visibility = Opened ? Visibility.Visible : Visibility.Collapsed;

            Border border = (Border)this.FindName("Border");
            if (Opened)
            {
                border.BorderBrush = Element.MinContentSize > Element.ChildElements.Count ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                border.BorderThickness = Element.MinContentSize > Element.ChildElements.Count ? new Thickness(2) : new Thickness(1);
            }
            else
            {
                border.BorderBrush = !IsOk() ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                border.BorderThickness = !IsOk() ? new Thickness(2) : new Thickness(1);
            }
            var addButton = (Button)this.FindName("AddButton");
            addButton.Visibility = Element.MaxContentSize != Element.ChildElements.Count ? Visibility.Visible : Visibility.Collapsed;
            addButton.ToolTip = "Add new child element to "+ Element.ToString();

            var toggleButton = (ToggleButton)this.FindName("ToggleButton");
            toggleButton.Visibility = Element.MaxContentSize == 0 ? Visibility.Collapsed : Visibility.Visible;

            var attributes = (GroupBox)this.FindName("AttributesGroupBox");
            attributes.Visibility = Element.Attributes.Count > 0 && Opened && Element.MaxContentSize != 0 ? Visibility.Visible : Visibility.Collapsed;

            var deleteButton = (Button)this.FindName("DeleteButton");
            deleteButton.ToolTip = "Delete "+ Element.ToString();
        }

        public bool IsOk()
        {
            foreach (ElementUserControl child in _childrenContainer.Children)
                if (!child.IsOk())
                    return false;
            return Element.MinContentSize <= Element.ChildElements.Count;
        }

        public void DeleteChildElement(IElement element)
        {
            Element.ChildElements.Remove(element);
            SetButtons();
            SetUpUI();
        }
        public void SetUpUI()
        {
            _unexpandableAttributesContainer.Children.Clear();
            if (Element.MaxContentSize == 0)
            {
                foreach (var attr in Element.Attributes)
                {
                    if (Element is GetValueElement)
                    {
                        _unexpandableAttributesContainer.Children.Add(new AttributeContainer(attr, true));
                        break;
                    }
                    else
                    {
                        _unexpandableAttributesContainer.Children.Add(new AttributeContainer(attr));
                    }
                }
            }
            else
            {
                _attributesContainer.Children.Clear();
                foreach (var attr in Element.Attributes)
                {
                    _attributesContainer.Children.Add(new AttributeContainer(attr));
                }
                _childrenContainer.Children.Clear();
                foreach (var child in Element.ChildElements)
                {
                    _childrenContainer.Children.Add(CreateDraggableElement(child));
                }
                _childrenContainer.Visibility = Visibility.Visible;
            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            IsMainLabelWithinViewport = true;
            Border dock = (Border)this.FindName("Border");
            if (_childrenContainer.Visibility == Visibility.Collapsed)
            {
                _childrenContainer.Visibility = Visibility.Visible;
                ((ToggleButton)sender).Content = "-"; // Change content to down arrow
                dock.HorizontalAlignment = HorizontalAlignment.Stretch;
                dock.MaxWidth = 1220;
                Label endLabel = (Label)this.FindName("EndLabel");
                endLabel.Visibility = Visibility.Visible;
                Opened = true;
            }
            else
            {
                _childrenContainer.Visibility = Visibility.Collapsed;
                ((ToggleButton)sender).Content = "+"; // Change content to right arrow
                dock.HorizontalAlignment = HorizontalAlignment.Left;
                Opened = false;
                Label endLabel = (Label)this.FindName("EndLabel");
                endLabel.Visibility = Visibility.Collapsed;
            }
            SetButtons();
        }
        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            if(Element is ICim)
            {
                MainWindow.RemoveCimClass(this);
            }
            else
            {
                StackPanel parentStack = this.Parent as StackPanel;
                Grid parentDock = parentStack.Parent as Grid;
                Grid parentDock2 = parentDock.Parent as Grid;
                Border parentBorder = parentDock2.Parent as Border;
                ElementUserControl parent = parentBorder.Parent as ElementUserControl;
                parent.DeleteChildElement(Element);
            }
        }
        public void XMLPreview_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.BindElementToXMLPreview(Element);
        }
        private void AddChildElement_Click(object sender, RoutedEventArgs e)
        {
            AddChildElementWindow window = new AddChildElementWindow(Element);
            if (window.ShowDialog() == true)
            {
                AddChildElement(window.SelectedOption);
            }
        }

        public void AddChildElement(string elementName)
        {
            string selectedElement = elementName;
            Element.AddChildElementToContent(ElementProviderReflection.CreateNewElement(selectedElement));
            SetUpUI();
            SetButtons();
        }

        private ElementUserControl CreateDraggableElement(IElement e)
        {
            var element = new ElementUserControl(e);
            element.MouseMove += Element_MouseMove;
            element.DragOver += Element_DragOver;
            element.Drop += Element_Drop;

            return element;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && sender is ElementUserControl element)
                {
                    _dragStartPoint = e.GetPosition(null);
                    _draggedElement = element;
                    _originalIndex = ChildrenContainer.Children.IndexOf(element);
                    DragDrop.DoDragDrop(element, element, DragDropEffects.Move);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Element_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void Element_Drop(object sender, DragEventArgs e)
        {
            if (_draggedElement != null && sender is ElementUserControl targetElement)
            {
                int targetIndex = ChildrenContainer.Children.IndexOf(targetElement);

                if (targetIndex != _originalIndex)
                {
                    ChildrenContainer.Children.Remove(_draggedElement);
                    ChildrenContainer.Children.Insert(targetIndex, _draggedElement);
                }
                _draggedElement = null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
