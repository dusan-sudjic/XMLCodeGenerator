using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using XMLCodeGenerator.Model.Blueprints;
using XMLCodeGenerator.Model.BuildingBlocks;
using XMLCodeGenerator.Model.BuildingBlocks.Abstractions;

namespace XMLCodeGenerator.Model
{
    public static class ElementFactory
    {
        public static IElement CreateElementFromBlueprint(ElementBlueprint blueprint)
        {
            if (blueprint == null)
                return null;
            IElement element = InitializeElement(blueprint);
            if(blueprint.ContentPattern.Length > 0) //if there is a content pattern
            {
                List<ChildrenPattern> childrenBlueprints = BlueprintsProvider.GetChildrenPatternsOfContentPattern(blueprint.ContentPattern);
                foreach (var childrenBlueprint in childrenBlueprints)
                {
                    for(int i = 0; i< childrenBlueprint.MinSize; i++)
                    {
                        IElement elem = CreateElementFromBlueprint(BlueprintsProvider.GetDefaultByInterface(childrenBlueprint.Interface));
                        if(elem!=null)
                            element.ChildElements.Add(elem);
                    }
                }
            }
            return element;
        }
        private static IElement InitializeElement(ElementBlueprint blueprint)
        {
            switch (blueprint.Interface.Name)
            {
                case "INumberExpression": return new NumberExpression(blueprint);
                case "ICondition": return new Condition(blueprint);
                case "IBooleanOperator": return new BooleanOperator(blueprint);
                case "IIf": return new If(blueprint);
                case "IElseIf": return new ElseIf(blueprint);
                case "IElse": return new Else(blueprint);
                case "IExpression": return new Expression(blueprint);
                case "ICimProperty": return new CimProperty(blueprint);
                case "ICimClass": return new CimClass(blueprint);
                case "IInputParameter": return new InputParameter(blueprint);
                case "IDefault": return new Default(blueprint);
                case "ICase": return new Case(blueprint);
                default: return null;
            }
        }

        private static Type getInterface(string _interface)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(_interface));
        }
    }
}
