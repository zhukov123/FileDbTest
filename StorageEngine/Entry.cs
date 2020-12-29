namespace StorageEngine
{
    public class Entry
    {
        public Entry(string key, string value)
        {
            Key = key;
            Value = value;
        }

        string Key { get; }
        string Value { get; }
    }
}