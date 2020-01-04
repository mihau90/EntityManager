using Caliburn.Micro;
using EntityManager.Enums;

namespace EntityManager.Models
{
    public class PropertyModel : Screen
    {
        private string _propertyName = string.Empty;
        private string _propertyType = string.Empty;
        private string _description = string.Empty;
        private string _defaultValue = string.Empty;
        private string _columnName = string.Empty;
        private string _columnType = string.Empty;
        private int _charLimit = 0;
        private RelationType _relation = RelationType.None;


        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                _propertyName = value.FirstCharToLower();
                NotifyOfPropertyChange(() => PropertyName);
            }
        }
        public string PropertyType
        {
            get { return _propertyType; }
            set
            {
                _propertyType = value.FirstCharToUpper();
                NotifyOfPropertyChange(() => PropertyType);
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                NotifyOfPropertyChange(() => DefaultValue);
            }
        }
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                _columnName = value.FirstCharToLower();
                NotifyOfPropertyChange(() => ColumnName);
            }
        }
        public string ColumnType
        {
            get { return _columnType; }
            set
            {
                _columnType = value.ToUpper();
                NotifyOfPropertyChange(() => ColumnType);
            }
        }
        public int CharLimit
        {
            get { return _charLimit; }
            set
            {
                _charLimit = value;
                NotifyOfPropertyChange(() => CharLimit);
            }
        }
        public RelationType Relation
        {
            get { return _relation; }
            set
            {
                _relation = value;
                NotifyOfPropertyChange(() => Relation);
            }
        }


        public bool IsGet { get; set; } = true;
        public bool IsSet { get; set; }
        public bool IsOptional { get; set; }


        public string ColumnDefinition
        {
            get
            {
                if (ColumnName == "id")
                    return "CHAR (38) NOT NULL PRIMARY KEY";

                var definition = ColumnType;

                if (CharLimit > 0)
                    definition += $"({ CharLimit })";

                if (!IsOptional)
                    definition += " NOT NULL";

                return definition.ToUpper();
            }
        }
        public bool IsCollection
        {
            get
            {
                if (string.IsNullOrEmpty(PropertyType))
                {
                    return false;
                }
                return PropertyType.EndsWith("[]");
            }
        }
        public bool IsAllAddInType
        {
            get
            {
                if (string.IsNullOrEmpty(PropertyType))
                {
                    return false;
                }
                return PropertyType.ToLower().StartsWith("allentities");
            }
        }
    }
}
