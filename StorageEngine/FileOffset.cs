namespace StorageEngine
{
    public class FileOffset
    {
        public FileOffset(string key, long startOffset, int length)
        {
            Key = key;
            StartOffset = startOffset;
            Length = length;
        }

        public string Key { get; }
        public long StartOffset { get; }
        public int Length { get; }
    }
}