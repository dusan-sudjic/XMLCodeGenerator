using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XMLCodeGenerator.Model;

namespace XMLCodeGenerator.ViewModel
{
    public class ElementViewModel: INotifyPropertyChanged
    {
        public string XML_Name { get => Element.Model.XMLName; set { } }
        public string Name { get=> Element.Model.Name; set { } }
        public bool HasAttributes { get => Element.Model.Attributes.Count > 0; }
        public bool HasEditableAttributes { get => Element.Model.Attributes.Where(a=>a.Editable).ToList().Count > 0; }
        public bool IsExtendable { get => Element.Model.ContentBlocks.Count > 0; set { } }
        public bool IsExtendedAndHasAttributes { get => IsExtended && HasAttributes; set { } }
        public bool IsExtendedAndHasEditableAttributes { get => IsExtended && HasEditableAttributes; set { } }
        private bool _isExtended;
        public bool IsExtended
        {
            get=>_isExtended && IsExtendable;
            set
            {
                if(value!=_isExtended)
                {
                    _isExtended = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _hasRoomForNewChildElement;
        public bool HasRoomForNewChildElement
        {
            get => _hasRoomForNewChildElement;
            set
            {
                if(value != _hasRoomForNewChildElement)
                {
                    _hasRoomForNewChildElement = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isRemovable;
        public bool IsRemovable
        {
            get => _isRemovable;
            set
            {
                if (value != _isRemovable)
                {
                    _isRemovable = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isReplacable;
        public bool IsReplacable
        {
            get => _isReplacable && !_isRemovable;
            set
            {
                if (value != _isReplacable)
                {
                    _isReplacable = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsReplacableOrRemovable
        {
            get => _isReplacable || _isRemovable;
            set{}
        }
        public bool IsFunctionDefinition { get=>Name.Contains("FunctionDefinition"); set { } }

        private ObservableCollection<AttributeViewModel> _attributes;
        public ObservableCollection<AttributeViewModel> Attributes
        {
            get { return _attributes; }
            set
            {
                _attributes = value;
                foreach (var attribute in _attributes)
                {
                    attribute.PropertyChanged += Attribute_PropertyChanged;
                }
            }
        }
        public ElementModel DefaultNewChild { get; set; }

        private string _additionalInfo;
        public string AdditionalInfo
        {
            get
            {
                if (Element.Model.Name.Equals("CimClass") || Element.Model.Name.Equals("CimProperty") || Element.Model.Name.Equals("FunctionDefinition"))
                    return "[" + Attributes.FirstOrDefault(a => a.Name.Equals("name", StringComparison.OrdinalIgnoreCase))?.Value + "]";
                if(Element.Model is FunctionModel functionModel)
                    return "[" + functionModel.FunctionName + "]";
                else return "";
            }
            set
            {
                _additionalInfo = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ElementViewModel> ChildViewModels = new();
        public Element Element { get; set; }
        
        public ElementViewModel(Element element)
        {
            Element = element;
            HasRoomForNewChildElement = ModelProvider.GetModelsForNewChildElement(Element).Count > 0;
            IsRemovable = XML_Name.Equals("CimClass") || Element.Model.Name.Equals("FunctionDefinition");
            IsReplacable = ModelProvider.GetReplacableModelsForElement(Element) != null;
            Attributes = new();
            var list = ModelProvider.GetModelsForNewChildElement(Element);
            DefaultNewChild = list.Count == 1 ? list[0] : null;
            IsExtended = true;
            Attributes.CollectionChanged += Attributes_CollectionChanged;
            foreach (var child in element.ChildElements)
                ChildViewModels.Add(new ElementViewModel(child));
            foreach(var attribute in element.Model.Attributes)
                Attributes.Add(new AttributeViewModel(attribute, element.AttributeValues[element.Model.Attributes.IndexOf(attribute)]));
            setRemovableForChildren();
        }
        public void SetReplacable()
        {
            IsReplacable = ModelProvider.GetReplacableModelsForElement(Element) != null;
            foreach (var child in ChildViewModels)
                child.SetReplacable();
        }
        public void AddNewChildElement(ElementModel model)
        {
            List<ElementModel> list;
            Element newElement = null;
            for (int i = Element.Model.ContentBlocks.Count - 1; i >= 0; i--)
            {
                if (Element.Model.ContentBlocks[i].SupportsElementModel(model))
                {
                    newElement = new Element(model, Element.Model.ContentBlocks[i]);
                    break;
                }
            }
            for(int i = 0; i<Element.ChildElements.Count; i++)
            {
                if (Element.Model.ContentBlocks.IndexOf(Element.ChildElements[i].ParentContentBlock) < Element.Model.ContentBlocks.IndexOf(newElement.ParentContentBlock))
                    continue;
                else if(Element.Model.ContentBlocks.IndexOf(Element.ChildElements[i].ParentContentBlock) > Element.Model.ContentBlocks.IndexOf(newElement.ParentContentBlock))
                {
                    Element.ChildElements.Insert(i, newElement);
                    ChildViewModels.Insert(i, new ElementViewModel(newElement));
                }
                else
                { 
                    while (Element.ChildElements.Count>i && Element.ChildElements[i].ParentContentBlock == newElement.ParentContentBlock) 
                        i++;
                    Element.ChildElements.Insert(i, newElement);
                    ChildViewModels.Insert(i, new ElementViewModel(newElement));
                }
                setRemovableForChildren();
                list = ModelProvider.GetModelsForNewChildElement(Element);
                DefaultNewChild = list.Count == 1 ? list[0] : null;
                return;
            }
            Element.ChildElements.Add(newElement);
            ChildViewModels.Add(new ElementViewModel(newElement));
            setRemovableForChildren();
            list = ModelProvider.GetModelsForNewChildElement(Element);
            DefaultNewChild = list.Count == 1 ? list[0] : null;
        }
        public void DeleteChildElement(ElementViewModel element)
        {
            ChildViewModels.Remove(element);
            Element.ChildElements.Remove(element.Element);
            setRemovableForChildren();
            var list = ModelProvider.GetModelsForNewChildElement(Element);
            DefaultNewChild = list.Count == 1 ? list[0] : null;
        }
        public void ReplaceChild(ElementViewModel oldElement, ElementModel newElementModel)
        {
            int index = ChildViewModels.IndexOf(oldElement);
            Element newElement = new Element(newElementModel, oldElement.Element.ParentContentBlock);
            ChildViewModels[index] = new ElementViewModel(newElement);
            Element.ChildElements[index] = ChildViewModels[index].Element;
            var list = ModelProvider.GetModelsForNewChildElement(Element);
            DefaultNewChild = list.Count == 1 ? list[0] : null;
        }
        public void RenameFunction(string oldName, string newName)
        {
            if (Element.Model.Name.Equals("FunctionCall"))
            {
                if (Attributes[0].Value.Equals(oldName))
                {
                    Element.Model = ModelProvider.GetFunctionModelByName(newName);
                    Attributes[0].Value = newName;
                }
            }
            else
                foreach (var c in ChildViewModels)
                    c.RenameFunction(oldName, newName);
        }
        private void setRemovableForChildren()
        {
            foreach(var child in ChildViewModels)
                child.IsRemovable = Element.ChildElements.Where(c=>c.ParentContentBlock==child.Element.ParentContentBlock).ToList().Count > child.Element.ParentContentBlock.MinSize;
        }
        private void Attributes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (AttributeViewModel newItem in e.NewItems)
                    newItem.PropertyChanged += Attribute_PropertyChanged;
            if (e.OldItems != null)
                foreach (AttributeViewModel oldItem in e.OldItems)
                    oldItem.PropertyChanged -= Attribute_PropertyChanged;
        }

        private void Attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                foreach (var attr in Attributes)
                {
                    int index = Element.Model.Attributes.IndexOf(attr.Attribute);
                    index += index >= 0 ? 0 : 1;
                    Element.AttributeValues[index] = attr.Value;
                }
                OnPropertyChanged(nameof(AdditionalInfo));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == "IsExtended")
            {
                OnPropertyChanged(nameof(IsExtendedAndHasAttributes));
                OnPropertyChanged(nameof(IsExtendedAndHasEditableAttributes));
            }
            if(!propertyName.Contains("IsExtended"))
                MainWindow.HasUnsavedChanges = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return XML_Name;
        }
    }
}
