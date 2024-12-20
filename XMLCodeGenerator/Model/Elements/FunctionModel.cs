﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCodeGenerator.Model.Elements
{
    public class FunctionModel : ElementModel
    {
        public string FunctionName { get; private set; }
        public int CallsCounter { get; set; }
        public FunctionModel(string functionName) : base()
        {
            FunctionName = functionName;
            CallsCounter = 0;
            Name = "FunctionCall";
            XMLName = "Function";
            ContentBlocks = new List<ContentBlockModel>();
            Attributes = new List<AttributeModel>();
            Attributes.Add(AttributeModel.CreateAttributeModelForFunctionCall(functionName));
        }
        public override ContentBlockModel GetSuitableContentBlockForChildModel(ElementModel model)
        {
            return null;
        }
        public override string ToString()
        {
            return Name + " [" + FunctionName + "]";
        }
    }
}
