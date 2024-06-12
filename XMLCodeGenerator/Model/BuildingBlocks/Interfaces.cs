using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLCodeGenerator.Model.BuildingBlocks
{
    public interface IElement 
    {
        List<IElement> ChildElements { get; set; }
        abstract int MinContentSize { get; }
        abstract int MaxContentSize { get; }
        bool AddChildElementToContent(IElement newElement);
        List<Type> SupportedChildElementTypes { get; }
        abstract string XML_Name { get; }
        HashSet<Attribute> Attributes { get; set; }
    }
    public interface ICim : IElement { }
    public interface ICimProperty : IElement { }
    public interface IExpression : IElement { }
    public interface ICondition : IElement { }
    public interface IBooleanOperator : IElement { }
    public interface INumberExpression : IExpression { }
    public interface ICase: IElement { }
    public interface IInputParameter: IElement { }
    public interface IDefault: IElement { }
}
