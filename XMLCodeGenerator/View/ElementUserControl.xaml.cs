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
using XMLCodeGenerator.View.Attributes;
using XMLCodeGenerator.ViewModel;

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
        private int _originalIndex;
        public ElementViewModel Element { get; set; }
        public ElementUserControl()
        {
            InitializeComponent();
        }
        public ElementUserControl(ElementViewModel element): this()
        {
            DataContext = this;
            Element = element;
            _childrenContainer = (StackPanel)this.FindName("ChildrenContainer");
            _attributesContainer = (StackPanel)this.FindName("AttributesStackPanel");
            _unexpandableAttributesContainer = (StackPanel)this.FindName("AttributesContainer");
            //StickyTextBlockBehavior.SetIsTopVisible(this, true);

            SetUpUI();
            SetButtons();
        }

        public void SetButtons()
        {
            var addButton = (Button)this.FindName("AddButton");
            if (Element.DefaultNewChild != null)
                addButton.ToolTip = "Add " + Element.DefaultNewChild;
            else
                addButton.ToolTip = "Add new element to " + Element.XML_Name;
            var deleteButton = (Button)this.FindName("DeleteButton");
            deleteButton.ToolTip = "Delete " + Element.ToString();
            var replaceButton = (Button)this.FindName("ReplaceButton");
            replaceButton.ToolTip = "Replace " + Element.ToString();
            var toggleButton = (ToggleButton)this.FindName("ToggleButton");
            toggleButton.Content = Element.IsExtended ? "-" : "+";
            Border dock = (Border)this.FindName("Border");
            dock.HorizontalAlignment = Element.IsExtended ? HorizontalAlignment.Stretch: HorizontalAlignment.Left;
            dock.HorizontalAlignment = !Element.IsExtendable ? HorizontalAlignment.Stretch: dock.HorizontalAlignment;
        }

        public void DeleteChildElement(ElementViewModel element)
        {
            Element.DeleteChildElement(element);
            SetButtons();
            SetUpUI();
        }
        public void ReplaceChildElement(ElementViewModel element)
        {
            AddChildElementWindow window = new AddChildElementWindow(element, true);
            if (window.ShowDialog() == true)
            {
                Element.ReplaceChild(element, window.SelectedElement);
                SetButtons();
                SetUpUI();
            }
            
        }
        public void SetUpUI()
        {
            _unexpandableAttributesContainer.Children.Clear();
            if (!Element.IsExtendable)
            {
                foreach (var attr in Element.Attributes)
                    if(attr.Attribute.Editable)
                        _unexpandableAttributesContainer.Children.Add(new AttributeContainer(attr));
            }
            else
            {
                _attributesContainer.Children.Clear();
                foreach (var attr in Element.Attributes)
                    if(attr.Attribute.Editable)
                        _attributesContainer.Children.Add(new AttributeContainer(attr));

                _childrenContainer.Children.Clear();
                foreach (var child in Element.ChildViewModels)
                    _childrenContainer.Children.Add(CreateDraggableElement(child));
            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            IsMainLabelWithinViewport = true;
            Border dock = (Border)this.FindName("Border");
            if (!Element.IsExtended)
            {
                ((ToggleButton)sender).Content = "-";
                dock.HorizontalAlignment = HorizontalAlignment.Stretch;
                //dock.MaxWidth = 1220;
                Element.IsExtended = true;
            }
            else
            {
                ((ToggleButton)sender).Content = "+";
                dock.HorizontalAlignment = HorizontalAlignment.Left;
                Element.IsExtended = false;
            }
            SetButtons();
        }
        private void DeleteElement_Click(object sender, RoutedEventArgs e)
        {

            if(Element.XML_Name.Equals("CimClass"))
            {
                MainWindow.RemoveCimClass(this);
            }
            else if (Element.Element.Model.Name.Equals("FunctionDefinition"))
            {
                MainWindow.RemoveFunctionDefinition(this);
            }
            else
            {
                ElementUserControl parent = GetParentElementUserControl();
                if (Element.IsRemovable)
                    parent.DeleteChildElement(Element);
                else
                    parent.ReplaceChildElement(Element);
            }
        }

        private ElementUserControl GetParentElementUserControl()
        {
            StackPanel parentStack = this.Parent as StackPanel;
            Grid parentDock = parentStack.Parent as Grid;
            Grid parentDock2 = parentDock.Parent as Grid;
            Border parentBorder = parentDock2.Parent as Border;
            ElementUserControl parent = parentBorder.Parent as ElementUserControl;
            return parent;
        }

        public void XMLPreview_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.BindElementToXMLPreview(Element);
        }
        private void AddChildElement_Click(object sender, RoutedEventArgs e)
        {
            if (Element.DefaultNewChild == null)
            {
                AddChildElementWindow window = new AddChildElementWindow(Element);
                if (window.ShowDialog() == true)
                    AddChildElement(window.SelectedElement);
            }
            else
            {
                AddChildElement(Element.DefaultNewChild);
            }
        }

        public void AddChildElement(ElementModel model)
        {
            Element.AddNewChildElement(model);
            SetUpUI();
            SetButtons();
        }

        private ElementUserControl CreateDraggableElement(ElementViewModel e)
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
