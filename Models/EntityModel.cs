namespace EntityManager.Models
{
    public class EntityModel
    {
        public string ShortName { get; set; }
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