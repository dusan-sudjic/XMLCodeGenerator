using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XMLCodeGenerator.Model.Blueprints;

namespace XMLCodeGenerator.Model.BuildingBlocks.Abstractions
{
    public interface IElement
    {
        string ContentPattern { get; init; }
        List<IElement> ChildElements { get; set; }
        abstract string XML_Name { get; init; }
        HashSet<Attribute> Attributes { get; set; }
    }
    public interface ICimClass : IElement { }
    public interface IIf : IExpression { }
    public interface IElse : IElement { }
    public interface ICimProperty : IElement { }
    public interface IExpression : IElement { }
    public interface ICondition : IElement { }
    public interface IBooleanOperator : IElement { }
    public interface INumberExpression : IExpression { }
    public interface ICase : IElement { }
    public interface IInputParameter : IElement { }
    public interface IDefault : IElement { }
}
