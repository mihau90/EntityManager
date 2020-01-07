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
    }
}
