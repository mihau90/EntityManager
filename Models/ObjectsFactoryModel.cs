using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityManager.Models
{
    public class ObjectsFactoryModel
    {
        public string VbName { get; set; }
        public string Description { get; set; }
        public List<FunctionModel> Functions { get; set; } = new List<FunctionModel>();

        public int GetMaxFunctionNameLenght() => Functions.Max(f => f.VbaName.Length);
        public int GetMaxFunctionDeclarationLenght() => Functions.Max(f => f.VbaDeclaration.Length);

        public ObjectsFactoryModel AddFunction(string functionType)
        {
            Functions.Add(new FunctionModel(functionType));
            return this;
        }

        public string ToVba()
        {
            var maxFunctionNameLenght = GetMaxFunctionNameLenght();
            var maxFunctionDeclarationLenght = GetMaxFunctionDeclarationLenght();

            var codeBlock = new List<string>() 
            {
                $"Attribute VB_Name = \"{ VbName }\"",
                Description,
                string.Empty,
                "Option Explicit",
                string.Empty,
            };

            var sortedList = Functions.OrderBy(x => x.Name).ToList();
            var unduplicatedList = sortedList.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            var vbaFunctionsList = sortedList.Select(f => f.ToVba(maxFunctionNameLenght, maxFunctionDeclarationLenght)).ToList();

            codeBlock.Add(string.Join(Environment.NewLine, vbaFunctionsList));

            return string.Join(Environment.NewLine, codeBlock);
        }
    }
}
