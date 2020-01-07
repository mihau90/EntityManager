using EntityManager.Models;
using System.Collections.Generic;

namespace EntityManager
{
    public class PropertyAutofiller
    {
        public Dictionary<string, string> AlladdinEntities { get; private set; }
        public EntityModel Entity { get; private set; }

        public PropertyAutofiller(Dictionary<string, string> alladdinEntities, EntityModel entity)
        {
            AlladdinEntities = alladdinEntities;
            Entity = entity;
        }

        public void AutoFill(PropertyModel property)
        {
            if (property.IsParent)
            {
                var parentType = Entity.FullName.Replace("ItemEntity", "Entity");
                property.SetParentProperties(parentType);
                return;
            }

            if (property.IsChild)
            {
                var childType = Entity.FullName.Replace("Entity", "ItemEntity[]"); ;
                property.SetChildProperties(childType);
                return;
            }

            if (AlladdinEntities.ContainsKey(property.PropertyName))
            {
                property.PropertyType = AlladdinEntities[property.PropertyName];
            }

            property.SetRelation()
                .SetPropertyType()
                .SetDefaultColumn()
                .SetPropertyDefaultValue();
        }
    }
}
