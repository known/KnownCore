using Known.Extensions;
using Known.Validation;
using System.Collections.Generic;

namespace Known.Mapping
{
    public class EntityBase
    {
        public EntityBase()
        {
            IsNew = true;
        }

        internal bool IsNew { get; set; }

        public Validator Validate()
        {
            var errors = new List<string>();
            var properties = GetType().GetColumnProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                var attr = property.GetAttribute<ColumnAttribute>();
                if (attr != null)
                    attr.Validate(value, errors);
            }
            return new Validator(errors);
        }
    }
}
