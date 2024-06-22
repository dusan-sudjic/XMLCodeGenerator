using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        private bool _isExtended;
        public bool IsExtended
        {
            get=>_isExtended;
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
        public ObservableCollection<ElementViewModel> ChildViewModels = new();
        public IElement Element { get; set; }
        
        public ElementViewModel(IElement element)
        {
            Element = element;
            HasRoomForNewChildElement = BlueprintsProvider.GetBlueprintsForNewChildElement(Element).Count > 0;
            IsRemovable = XML_Name.Equals("CimClass");
            IsReplacable = BlueprintsProvider.GetReplacementBlueprintsForElement(Element).Count > 0;
            foreach (var child in element.ChildElements)
                ChildViewModels.Add(new ElementViewModel(child));
        }
        public void AddNewChildElement(IElement e)
        {
            for(int i=Element.ChildElements.Count; i>= 0; i--)
            {
                if(i<Element.ChildElements.Count)
                    Element.ChildElements.Insert(i, e);
                else if (i == Element.ChildElements.Count)
                    Element.ChildElements.Add(e);
                if (!BlueprintsProvider.ElementMatchesPattern(Element))
                {
                    Element.ChildElements.RemoveAt(i);
                    continue;
                }
                HasRoomForNewChildElement = BlueprintsProvider.GetBlueprintsForNewChildElement(Element).Count > 0;
                ReloadChildren();
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
        private void ReloadChildren()
        {
            ChildViewModels.Clear();
            foreach (var child in Element.ChildElements)
                ChildViewModels.Add(new ElementViewModel(child));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return XML_Name;
        }
    }
}
