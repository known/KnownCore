namespace Known.Cache
{
    public interface ICache
    {
        object Get(string key);
        void Set(string key, object value);
        void Set(string key, object value, int expires);
        void Remove(string key);
        void RemoveAll();
    }
}
