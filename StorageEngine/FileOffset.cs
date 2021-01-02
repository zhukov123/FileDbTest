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


        public string Key { get; set; }
        public long StartOffset { get; set; }
        public int Length { get; set; }
    }
}