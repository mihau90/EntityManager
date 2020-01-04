using Caliburn.Micro;
using EntityManager.Enums;

namespace EntityManager.Models
{
    public class PropertyModel : Screen
    {
        private string _propertyName = string.Empty;
        private string _propertyType = string.Empty;
        private string _description = string.Empty;
        private string _propertyDefaultValue = string.Empty;
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
        public string PropertyDefaultValue
        {
            get { return _propertyDefaultValue; }
            set
            {
                _propertyDefaultValue = value;
                NotifyOfPropertyChange(() => PropertyDefaultValue);
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
                if (!IsAllAddInType)
                {
                    _relation = RelationType.None;
                }
                else
                {
                    _relation = value;
                }

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
                return PropertyType.EndsWith("[]");
            }
        }
        public bool IsAllAddInType
        {
            get
            {
                return PropertyType.ToLower().StartsWith("allentities");
            }
        }
        public bool IsParent
        {
            get
            {
                if (PropertyName == "parent")
                    return true;

                return false;
            }
        }
        public bool IsChild
        {
            get
            {
                if (PropertyName == "items")
                    return true;

                return false;
            }
        }

        public PropertyModel SetRelation()
        {
            if (!IsAllAddInType)
            {
                Relation = RelationType.None;
                return this;
            }

            if (IsChild)
            {
                Relation = RelationType.OneToMany;
                return this;
            }

            Relation = RelationType.ManyToOne;
            return this;
        }
        public PropertyModel SetPropertyDefaultValue()
        {
            if (string.IsNullOrEmpty(PropertyType))
            {
                return this;
            }

            if (IsCollection)
            {
                PropertyDefaultValue = "New VBA.Collection";
                return this;
            }

            if (IsAllAddInType)
            {
                PropertyDefaultValue = "Nothing";
                return this;
            }

            switch (PropertyType)
            {
                case "Integer":
                case "Long":
                    PropertyDefaultValue = "0";
                    break;

                case "Double":
                    PropertyDefaultValue = "0#";
                    break;

                case "Date":
                    PropertyDefaultValue = "Null";
                    break;

                case "String":
                    PropertyDefaultValue = "vbNullString";
                    break;
            }

            return this;

        }
        public PropertyModel SetDefaultColumn()
        {
            if (string.IsNullOrEmpty(PropertyType))
            {
                return this;
            }

            if (PropertyName == "id")
            {
                ColumnName = "id";
                ColumnType = "CHAR";
                CharLimit = 38;
                return this;
            }

            if (IsAllAddInType)
            {
                ColumnName = $"{PropertyName}_id";
                ColumnType = "CHAR";
                CharLimit = 38;
                return this;
            }

            if (IsChild)
            {
                ColumnName = "id";
            }
            else
            {
                ColumnName = PropertyName.ToSnakeCase();

            }

            switch (PropertyType.ToLower())
            {
                case "long":
                case "integer":
                    ColumnType = "INT";
                    CharLimit = 0;
                    break;

                case "date":
                    ColumnType = "DATETIME";
                    CharLimit = 0;
                    break;

                case "double":
                    ColumnType = "DOUBLE";
                    CharLimit = 0;
                    break;

                case "string":
                    ColumnType = "VARCHAR";
                    CharLimit = 50;
                    break;
            }

            return this;
        }
        public PropertyModel SetPropertyType()
        {
            if (string.IsNullOrEmpty(PropertyType))
            {
                return this;
            }

            switch (PropertyType.ToLower())
            {
                case "str":
                case "st":
                case "s":
                    PropertyType = "string";
                    break;

                case "int":
                case "i":
                case "l":
                    PropertyType = "long";
                    break;

                case "dt":
                    PropertyType = "date";
                    break;

                case "d":
                case "dbl":
                    PropertyType = "double";
                    break;
            }

            return this;
        }
        public PropertyModel SetParentProperties(string propertyType)
        {
            PropertyType = propertyType;
            SetRelation();
            SetDefaultColumn();
            SetPropertyDefaultValue();
            return this;
        }
        public PropertyModel SetChildProperties(string propertyType)
        {
            PropertyType = propertyType;
            SetRelation();
            SetDefaultColumn();
            SetPropertyDefaultValue();
            return this;
        }
    }
}
