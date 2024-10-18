using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using XMLCodeGenerator.Model.Elements;

namespace XMLCodeGenerator.ViewModel
{
    public class ElementViewModel : INotifyPropertyChanged
    {
        public string XMLName { get => Element.XMLName; set { } }
        public string Name { get => Element.Name; set { } }
        public bool IsNotWrapperElement { get =>!String.IsNullOrEmpty(XMLName); set { } }
        public bool IsExtendedAndIsNotWrapperElement { get => IsNotWrapperElement && IsExtended; set { } }
        public bool IsExtendableAndIsNotWrapperElement { get=> IsExtendable && IsNotWrapperElement; set { } }
        public ContentBlockModel ParentContentBlock { get => Element.ParentContentBlock; }
        public bool HasAttributes { get => Element.Model.Attributes.Count > 0; }
        public bool HasEditableAttributes { get => Element.Model.Attributes.Where(a => a.Editable).ToList().Count > 0; }
        public bool IsUnextendableAndHasEditableAttributes { get => !IsExtendable && HasEditableAttributes; }
        public bool IsExtendable { get => Element.Model.ContentBlocks.Count > 0; set { } }
        public bool IsExtendedAndHasAttributes { get => IsExtended && HasAttributes; set { } }
        public bool IsMovable { get => IsMovableUp || IsMovableDown; set { } }
        public bool IsClassMappingEnabled { get => Element.Model.ClassMappingEnabled; set { } }
        public bool IsMovableUp { 
            get
            {
                if (ParentContentBlock.MaxSize == 1)
                    return false;
                int index = Parent.ChildViewModels.IndexOf(this);
                if (index <= 0)
                    return false;
                if (Parent.ChildViewModels[index - 1].ParentContentBlock != ParentContentBlock)
                    return false;
                return true;
            }
            set { } 
        }
        public bool IsMovableDown
        {
            get
            {
                if (ParentContentBlock.MaxSize == 1)
                    return false;
                int index = Parent.ChildViewModels.IndexOf(this);
                if (index == Parent.ChildViewModels.Count-1)
                    return false;
                if (Parent.ChildViewModels[index + 1].ParentContentBlock != ParentContentBlock)
                    return false;
                return true;
            }
            set { }
        }
        public bool IsExtendedAndHasEditableAttributes { get => IsExtended && HasEditableAttributes; set { } }
        private bool _isExtended = true;
        public bool IsExtended
        {
            get => _isExtended && IsExtendable;
            set
            {
                if (value != _isExtended)
                {
                    _isExtended = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (value != _isHighlighted)
                {
                    _isHighlighted = value;
                    if (!value && IsSelected)
                        IsSelected = false;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set
            {
                if (value != _isNew)
                {
                    _isNew = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _hasRoomForNewChildElement;
        public bool HasRoomForNewChildElement
        {
            get => _hasRoomForNewChildElement && IsExtended;
            set
            {
                if (value != _hasRoomForNewChildElement)
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
                    OnPropertyChanged("IsReplacable");
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
            set { }
        }
        public bool IsFunctionDefinition { get => Name.Contains("FunctionDefinition"); set { } }
        public ElementViewModel Parent { get; init; }

        private ObservableCollection<AttributeViewModel> _attributes = new();
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
        public string AdditionalInfo
        {
            get
            {
                if (Name.Equals("CimClass") || Name.Equals("CimProperty") || Name.Equals("CimRelationship")
                    || Name.Equals("FunctionDefinition") || Name.Equals("PreProcessProcedure") || Name.Equals("RewritingProcedure"))
                    return "[" + Attributes[0].Value + "]";
                if (Element.Model is FunctionModel functionModel)
                    return "[" + functionModel.FunctionName + "]";
                else return "";
            }
            set{}
        }
        public string FunctionCalls
        {
            get
            {
                if (Name.Equals("FunctionDefinition"))
                {
                    int calls = ElementModelProvider.GetFunctionModelByName(Attributes[0].Value).CallsCounter;
                    return "  " + calls.ToString() + (calls==1 ?" call":" calls");
                }
                else return "";
            }
            set{}
        }
        private ObservableCollection<ElementViewModel> _childViewModels = new();
        public ObservableCollection<ElementViewModel> ChildViewModels
        {
            get => _childViewModels;
            set
            {
                _childViewModels = value;
                OnPropertyChanged(nameof(ChildViewModels));
            }
        }
        public Element Element { get; set; }

        public ElementViewModel(Element element, ElementViewModel parent = null)
        {
            Element = element;
            Parent = parent;
            HasRoomForNewChildElement = ElementModelProvider.GetModelsForNewChildElement(Element).Count > 0;
            IsReplacable = ElementModelProvider.GetReplacableModelsForElement(Element) != null;
            Attributes.CollectionChanged += Attributes_CollectionChanged;
            foreach (var child in element.ChildElements)
                ChildViewModels.Add(new ElementViewModel(child, this));
            foreach (var attribute in element.Model.Attributes)
                Attributes.Add(new AttributeViewModel(attribute, element.AttributeValues[element.Model.Attributes.IndexOf(attribute)], this));
            SetRemovableForChildren();
            if (Element.Model is FunctionModel fun)
            {
                ElementModelProvider.AddFunctionCall(fun.FunctionName);
                MainWindow.UpdateFunctionCallsCounter(fun.FunctionName);
            }
            if (ParentContentBlock == null) return;
            
        }
        public void RenameFirstElementInContentBlock(ContentBlockModel contentBlock)
        {
            foreach(var c in ChildViewModels)
            {
                if (c.ParentContentBlock == contentBlock)
                {
                    c.setFirstInContentBlock();
                    return;
                }
            }
        }
        public void SetReplacable()
        {
            var hasReplacableModels = ElementModelProvider.GetReplacableModelsForElement(Element) != null;
            var copiedElementCanReplaceCurrent = Element.ParentContentBlock.ElementModels.Contains(MainWindow.Document.Clipboard.Model);
            IsReplacable = hasReplacableModels && copiedElementCanReplaceCurrent;
            foreach (var child in ChildViewModels)
                child.SetReplacable();
        }
        public ElementViewModel AddNewChildElement(Element element)
        {
            element.ParentContentBlock = Element.Model.GetSuitableContentBlockForChildModel(element.Model);
            for (int i = 0; i < Element.ChildElements.Count; i++)
            {
                if (Element.Model.ContentBlocks.IndexOf(Element.ChildElements[i].ParentContentBlock) < Element.Model.ContentBlocks.IndexOf(element.ParentContentBlock))
                    continue;
                else if (Element.Model.ContentBlocks.IndexOf(Element.ChildElements[i].ParentContentBlock) > Element.Model.ContentBlocks.IndexOf(element.ParentContentBlock))
                    element.setFirstInContentBlock();
                else
                    while (Element.ChildElements.Count > i && Element.ChildElements[i].ParentContentBlock == element.ParentContentBlock)
                        i++;
                Element.ChildElements.Insert(i, element);
                ElementViewModel newElementViewModel = new ElementViewModel(element, this);
                ChildViewModels.Insert(i, newElementViewModel);
                SetRemovableForChildren();
                if (!newElementViewModel.IsMovableUp)
                    newElementViewModel.setFirstInContentBlock();
                else
                    newElementViewModel.setNotFirstInContentBlock();
                return newElementViewModel;
            }
            element.setFirstInContentBlock();
            Element.ChildElements.Add(element);
            ElementViewModel newElementVM = new ElementViewModel(element, this);
            ChildViewModels.Add(newElementVM);
            SetRemovableForChildren();
            if (!newElementVM.IsMovableUp)
                newElementVM.setFirstInContentBlock();
            else
                newElementVM.setNotFirstInContentBlock();
            return newElementVM;
        }
        public void DeleteElement()
        {
            if (Element.Model is FunctionModel)
            {
                ElementModelProvider.DeleteFunctionCall((Element.Model as FunctionModel).FunctionName);
                MainWindow.UpdateFunctionCallsCounter(Element.AttributeValues[0]);
            }
            Parent.ChildViewModels.Remove(this);
            Parent.Element.ChildElements.Remove(Element);
            Parent.SetRemovableForChildren();
            RefreshMovable();
            Parent.RenameFirstElementInContentBlock(ParentContentBlock);
        }
        public ElementViewModel ReplaceElement(Element newElement, bool keepingContent = false)
        {
            if(this.Element.Model is FunctionModel) 
            {
                ElementModelProvider.DeleteFunctionCall((Element.Model as FunctionModel).FunctionName);
                MainWindow.UpdateFunctionCallsCounter(Element.AttributeValues[0]);
            }
            int index = Parent.ChildViewModels.IndexOf(this);
            newElement.ParentContentBlock = ParentContentBlock;
            if (keepingContent)
                newElement.AddChildren(Element.ChildElements);
            ElementViewModel newElementViewModel = new ElementViewModel(newElement, Parent);
            Parent.ChildViewModels[index] = newElementViewModel;
            Parent.Element.ChildElements[index] = Parent.ChildViewModels[index].Element;
            if (!Parent.ChildViewModels[index].IsMovableUp)
                newElementViewModel.setFirstInContentBlock();
            else
                newElementViewModel.setNotFirstInContentBlock();
            return newElementViewModel;
        }
        public void RenameFunction(string oldName, string newName)
        {
            if (Name.Equals("FunctionCall"))
            {
                if (Attributes[0].Value.Equals(oldName))
                {
                    Element.Model = ElementModelProvider.GetFunctionModelByName(newName);
                    Attributes[0].Value = newName;
                }
            }
            else
                foreach (var c in ChildViewModels)
                    c.RenameFunction(oldName, newName);
        }
        public void SetRemovableForChildren()
        {
            foreach (var child in ChildViewModels)
                child.IsRemovable = Element.ChildElements.Where(c => c.ParentContentBlock == child.ParentContentBlock).ToList().Count > child.ParentContentBlock.MinSize;
        }
        public void MoveUp()
        {
            int index = Parent.ChildViewModels.IndexOf(this);
            Parent.ChildViewModels.Move(index, index - 1);
            var temp = Parent.Element.ChildElements[index];
            Parent.Element.ChildElements[index] = Parent.Element.ChildElements[index - 1];
            Parent.Element.ChildElements[index - 1] = temp;
            if (!Parent.ChildViewModels[index - 1].IsMovableUp)
            {
                Parent.ChildViewModels[index - 1].setFirstInContentBlock();
                Parent.ChildViewModels[index].setNotFirstInContentBlock();
            }
            RefreshMovable();
        }
        public void setFirstInContentBlock()
        {
            Element.setFirstInContentBlock();
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(XMLName));
        }
        public void setNotFirstInContentBlock()
        {
            Element.resetToModelsNames();
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(XMLName));
        }
        private void RefreshMovable()
        {
            if(Parent!=null)
                foreach (var e in Parent.ChildViewModels)
                {
                    e.OnPropertyChanged(nameof(IsMovableDown));
                    e.OnPropertyChanged(nameof(IsMovableUp));
                    e.OnPropertyChanged(nameof(IsMovable));
                }
        }

        public void MoveDown()
        {
            int index = Parent.ChildViewModels.IndexOf(this);
            Parent.ChildViewModels.Move(index, index + 1);
            var temp = Parent.Element.ChildElements[index];
            Parent.Element.ChildElements[index] = Parent.Element.ChildElements[index + 1];
            Parent.Element.ChildElements[index + 1] = temp;
            if (!Parent.ChildViewModels[index].IsMovableUp)
            {
                Parent.ChildViewModels[index].setFirstInContentBlock();
                Parent.ChildViewModels[index+1].setNotFirstInContentBlock();
            }
            RefreshMovable();
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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName.Contains("Movable"))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return;
            }
            if (propertyName == "IsExtended")
            {
                OnPropertyChanged(nameof(HasRoomForNewChildElement));
                OnPropertyChanged(nameof(IsExtendedAndHasAttributes));
                OnPropertyChanged(nameof(IsExtendedAndHasEditableAttributes));
                OnPropertyChanged(nameof(IsExtendedAndIsNotWrapperElement));
            }
            if (!propertyName.Contains("IsExtended") && !propertyName.Contains("Room") && !propertyName.Equals("IsHighlighted") && !propertyName.Equals("IsSelected"))
                MainWindow.Document.HasUnsavedChanges = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            RefreshMovable();
        }
        public List<ElementViewModel> SearchElement(string[] parameters, List<ElementViewModel> resultsSoFar = null, bool skip = false)
        {
            if (resultsSoFar == null) resultsSoFar = new List<ElementViewModel>();
            if(!skip && IsNotWrapperElement && parameters.All(p => ToString().ToLower().Contains(p.ToLower())))
            {
                resultsSoFar.Add(this);
            }
            foreach(var child in ChildViewModels)
            {
                child.SearchElement(parameters, resultsSoFar);
            }
            return resultsSoFar;
        }
        public string ListedAttributes
        {
            get
            {
                string ret = "[";
                foreach (var attr in Attributes)
                    ret += attr.ToString() + ", ";
                if (Attributes.Any())
                    ret = ret.Substring(0, ret.Length - 2);
                return ret + "]";
            }
            set { }
        }
        public override string ToString()
        {
            return $"{Name}{ListedAttributes}";
        }
    }
}