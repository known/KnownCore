using System;
using System.ComponentModel;
using System.Reflection;

namespace Known.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var attr = GetAttribute<DescriptionAttribute>(field, false);
            return attr != null ? attr.Description : name;
        }

        private static T GetAttribute<T>(MemberInfo member, bool inherit = true)
        {
            foreach (var attr in member.GetCustomAttributes(inherit))
            {
                if (attr is T)
                {
                    return (T)attr;
                }
            }
            return default(T);
        }
    }
}
