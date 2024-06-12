using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XMLCodeGenerator.Model.BuildingBlocks;

namespace XMLCodeGenerator.Model
{
    public static class ElementProviderReflection
    {
        //koristi dictionary za ovo
        public static IElement CreateNewElement(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type type = FindTypeByName(name, assembly);
            if (type == null)
                throw new Exception("Element " + name + " not found!");
            if (!typeof(Element).IsAssignableFrom(type))
                throw new Exception("Type " + name + " does not inherit from Element!");
            return (IElement)Activator.CreateInstance(type);
        }
        public static List<string> GetSupportedChildElements(IElement element)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> interfaceTypes = element.SupportedChildElementTypes;
            List<string> classNames = new List<string>();

            foreach (Type type in assembly.GetTypes())
                if (interfaceTypes.Any(i=>i.IsAssignableFrom(type)) && type.IsClass && !type.IsAbstract && type.IsSealed)
                        classNames.Add(RemoveElementSuffix(type.Name));
            classNames.Sort();
            return classNames;
        }
        private static Type FindTypeByName(string typeName, Assembly assembly)
        {
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(typeName+"Element"));
        }
        private static string RemoveElementSuffix(string input)
        {
            const string suffix = "Element";
            if (input.EndsWith(suffix))
                return input.Substring(0, input.Length - suffix.Length);
            return null;
        }
    }
}
