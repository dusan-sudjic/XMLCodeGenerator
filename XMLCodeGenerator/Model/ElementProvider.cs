using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XMLCodeGenerator.Model.BuildingBlocks;
using XMLCodeGenerator.Model.Elements;
using XMLCodeGenerator.Model.Elements.BooleanOperators;
using XMLCodeGenerator.Model.Elements.BooleanOperators.StringBooleanOperators;
using XMLCodeGenerator.Model.Elements.Conditions;
using XMLCodeGenerator.Model.Elements.GetOperators;
using XMLCodeGenerator.Model.Elements.MathOperators;
using XMLCodeGenerator.Model.Elements.StringOperators;

namespace XMLCodeGenerator.Model
{
    public sealed class ElementProvider
    {
        public static IElement CreateNewElement(string name)
        {
            return Mapper[name]; //kloniraj instancu, istrazi
        }
        public static List<string> GetSupportedChildElements(IElement element)
        {
            List<Type> interfaceTypes = element.SupportedChildElementTypes;
            List<string> classNames = new List<string>();

            foreach (var d in Mapper)
            {
                if (interfaceTypes.Any(i => i.IsAssignableFrom(d.Value.GetType())) && d.Value.GetType().IsClass && !d.Value.GetType().IsAbstract)
                    classNames.Add(d.Key);
            }
            classNames.Sort();
            return classNames;
        }
        private static Dictionary<string, IElement> Mapper = new Dictionary<string, IElement>
        {
            { "FunctionCall", FunctionCallElement.CreateNewInstance() },
            { "SetDefault", new SetDefaultElement() },
            { "BreakInstanceCreation", new BreakInstanceCreationElement() },
            { "Constant", new ConstantElement() },
            { "CimRelationship", new CimRelationshipElement() },
            { "CimProperty", new CimPropertyElement() },
            { "CimClass", new CimClassElement() },
            //string operators
            { "StringConcatenate", new StringConcatenateElement() },
            { "StringFormat", new StringFormatElement() },
            { "StringIndexOf", new StringIndexOfElement() },
            { "StringLastIndexOf", new StringLastIndexOfElement() },
            { "StringLength", new StringLengthElement() },
            { "StringRemove", new StringRemoveElement() },
            { "StringReplace", new StringReplaceElement() },
            { "StringSplit", new StringSplitElement() },
            { "StringToLower", new StringToLowerElement() },
            { "StringToUpper", new StringToUpperElement() },
            { "StringTrim", new StringTrimElement() },
            { "StringTrimEnd", new StringTrimEndElement() },
            { "StringTrimStart", new StringTrimStartElement() },
            { "Substring", new SubstringElement() },
            //math operators
            { "Addition", new UnlimitedMathOperator("Addition") },
            { "Substraction", new BinaryMathOperation("Substraction") },
            { "Division", new BinaryMathOperation("Division") },
            { "Multiplication", new UnlimitedMathOperator("Multiplication") },
            { "MathPow", new BinaryMathOperation("MathPow") },
            { "AbsoluteValue", new UnaryMathOperation("AbsoluteValue") },
            { "MathMax", new BinaryMathOperation("MathMax") },
            { "MathMin", new BinaryMathOperation("MathMin") },
            { "MathSqrt", new UnaryMathOperation("MathSqrt") },
            { "MathCeiling", new UnaryMathOperation("MathCeiling") },
            { "MathFloor", new UnaryMathOperation("MathFloor") },
            { "MathRound", new UnaryMathOperation("MathRound") },
            { "MathTruncate", new UnaryMathOperation("MathTruncate") },
            { "MathCos", new UnaryMathOperation("MathCos") },
            { "MathSin", new UnaryMathOperation("MathSin") },
            { "MathTan", new UnaryMathOperation("MathTan") },
            //conditions
            { "if", new IfElement() },
            { "AndConditions", new AndConditionsElement() },
            { "OrConditions", new OrConditionsElement() },
            { "Condition", new ConditionElement() },
            { "Case", new CaseElement() },
            { "Default", new DefaultElement() },
            { "InputParameter", new InputParameterElement() },
            { "SwitchCase", new SwitchCaseElement() },
            //get operators
            { "GetValue", new GetValueElement() },
            { "GetPrimarySource", new GetPrimarySourceElement() },
            { "GetSourceColumn", new GetSourceColumnElement() },
            { "GetSourceTable", new GetSourceTableElement() },
            //boolean operators
            { "GreaterThan", new NumericComparator("GreaterThan") },
            { "GreaterThanOrEqual", new NumericComparator("GreaterThanOrEqual") },
            { "LessThan", new NumericComparator("LessThan") },
            { "LessThanOrEqual", new NumericComparator("LessThanOrEqual")},
            { "StringContains", new StringContainsElement() },
            { "StringEndsWith", new StringEndsWithElement() },
            { "StringStartsWith", new StringStartsWithElement() },
            { "StringIsNullOrEmpty", new StringIsNullOrEmptyElement() },
            { "StringIsNullOrWhiteSpace", new StringIsNullOrWhiteSpaceElement() },
            { "AreEqual", new AreEqualElement() },
            { "ContainsProperty", new ContainsPropertyElement() },
            { "IsFalse", new IsFalseElement() },
            { "IsTrue", new IsTrueElement() },
            { "IsInConcreteModel", new IsInConcreteModelElement() },
            { "IsNull", new IsNullElement() },
            { "IsSource", new IsSourceElement() }
        };
    }
}
