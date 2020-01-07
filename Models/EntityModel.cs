namespace EntityManager.Models
{
    public class EntityModel
    {
        private string _shortName = string.Empty;
        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value.FirstCharToUpper(); }
        }
        public string LongName
        {
            get
            {
                return $"{ ShortName }Entity";
            }
        }
        public string FullName
        {
            get
            {
                return $"AllEntities.{ LongName }";
            }
        }
        public string Repository
        {
            get
            {
                return $"{ ShortName }Repository";
            }
        }
    }
}