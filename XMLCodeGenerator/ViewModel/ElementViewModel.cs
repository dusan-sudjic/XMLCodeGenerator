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
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.ViewModel
{
    public class ElementViewModel: INotifyPropertyChanged
    {
        public string XML_Name { get => Element.XML_Name; set { } }
        public bool HasAttributes { get => Element.Attributes.Count > 0; }
        public bool IsExtendable { get => Element.ContentPattern.Length > 0; set { } }
        public bool IsExtendedAndHasAttributes { get => IsExtended && HasAttributes; set { } }
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

        private string _additionalInfo;
        public string AdditionalInfo
        {
            get
            {
                if (Element is ICimClass || Element is ICimProperty)
                    return "[" + Attributes.FirstOrDefault(a => a.Name == "name")?.Value + "]";
                else return "";
            }
            set
            {
                _additionalInfo = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ElementViewModel> ChildViewModels = new();
        public IElement Element { get; set; }
        
        public ElementViewModel(IElement element)
        {
            Element = element;
            HasRoomForNewChildElement = BlueprintsProvider.GetBlueprintsForNewChildElement(Element).Count > 0;
            IsRemovable = XML_Name.Equals("CimClass");
            IsReplacable = BlueprintsProvider.GetReplacementBlueprintsForElement(Element).Count > 0;
            Attributes = new();
            IsExtended = true;
            Attributes.CollectionChanged += Attributes_CollectionChanged;
            foreach (var child in element.ChildElements)
                ChildViewModels.Add(new ElementViewModel(child));
            foreach(var attribute in element.Attributes)
                Attributes.Add(new AttributeViewModel(attribute));
        }
        public void AddNewChildElement(IElement e)
        {
            var newViewModel = new ElementViewModel(e);
            for(int i=ChildViewModels.Count; i>= 0; i--)
            {
                if (i < ChildViewModels.Count)
                {
                    Element.ChildElements.Insert(i, e);
                    ChildViewModels.Insert(i, newViewModel);
                }
                else if (i == Element.ChildElements.Count)
                {
                    Element.ChildElements.Add(e);
                    ChildViewModels.Add(newViewModel);
                }
                if (!BlueprintsProvider.ElementMatchesPattern(Element))
                {
                    Element.ChildElements.RemoveAt(i);
                    ChildViewModels.RemoveAt(i);
                    continue;
                }
                HasRoomForNewChildElement = BlueprintsProvider.GetBlueprintsForNewChildElement(Element).Count > 0;
                setRemovableForChildren();
                return;
            }
        }
        public void DeleteChildElement(ElementViewModel element)
        {
            ChildViewModels.Remove(element);
            Element.ChildElements.Remove(element.Element);
            setRemovableForChildren();
        }
        public void ReplaceChild(ElementViewModel oldElement, string newElement)
        {
            int index = ChildViewModels.IndexOf(oldElement);
            ChildViewModels[index] = new ElementViewModel(ElementFactory.CreateElementFromBlueprint(BlueprintsProvider.GetBlueprint(newElement)));
            Element.ChildElements[index] = ChildViewModels[index].Element;
        }
        private void setRemovableForChildren()
        {
            List<ChildrenPattern> children = BlueprintsProvider.GetChildrenPatternsOfContentPattern(Element.ContentPattern);
            foreach(var pattern in children)
            {
                List<ElementViewModel> viewModels = ChildViewModels.Where(e => pattern.Interface.IsAssignableFrom(BlueprintsProvider.GetBlueprint(e.XML_Name).Interface)).ToList();
                foreach(var a in viewModels)
                    a.IsRemovable = viewModels.Count > pattern.MinSize;
                viewModels.Clear();
            }
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
                OnPropertyChanged(nameof(AdditionalInfo));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == "IsExtended")
                OnPropertyChanged(nameof(IsExtendedAndHasAttributes));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return XML_Name;
        }
    }
}
