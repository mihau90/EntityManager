namespace EntityManager.Models
{
    public class FunctionModel
    {
        private string _type;
        public string Type
        {
            get { return _type; }
            set 
            { 
                Name = value.Split('.')[1];
                _type = value; 
            }
        }
        public string Name { get; private set; }

        public FunctionModel(string functionType)
        {
            this.Type = functionType;
        }

        public string ToVba()
        {
            return $"Public Function new{ Name }() As AllInterfaces.FactorableInterface: Set { Name } = New { Type }: End Function";
        }

    }
}