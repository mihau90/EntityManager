namespace EntityManager.Models
{
    public class FunctionModel
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string VbaName { get; private set; }
        public string VbaDeclaration { get; private set; }

        public FunctionModel(string functionType)
        {
            this.Type = functionType;
            Name = Type.Split('.')[1];
            VbaName = $"Public Function new{ Name }() As AllInterfaces.FactorableInterface:";
            VbaDeclaration = $"Set { Name } = New { Type }:";
        }

        public string ToVba(int totalNameLen = 0, int totalDeclarationLen = 0 )
        {
            var paddedName = VbaName.PadRight(totalNameLen);
            var paddedDeclaration = VbaDeclaration.PadRight(totalDeclarationLen);

            return $"{ paddedName }{ paddedDeclaration } End Function";
        }
    }
}