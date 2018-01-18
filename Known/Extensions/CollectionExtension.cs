using System.Collections.Generic;
using System.Collections.Specialized;

namespace Known.Extensions
{
    public static class CollectionExtension
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            if (collection == null)
                return null;

            var dict = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                dict.Add(key, collection[key]);
            }
            return dict;
        }

        public static T Value<T>(this IDictionary<string, object> dictionary, string key, T defValue = default(T))
        {
            if (dictionary.ContainsKey(key))
            {
                return (T)dictionary[key];
            }

            return defValue;
        }
    }
}
