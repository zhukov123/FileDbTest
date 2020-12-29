using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace Database_Testing_Console
{
    interface IFileDb
    {
        string Get(string collection, string key);
        bool AddOrUpdate(string collection, string key, string value);

    }

    class FileDb : IFileDb
    {
        private readonly string FilePath = "./";
        private readonly string DataFilePrefix = "Data_";
        private readonly string OffsetFilePrefix = "Offset_";

        private ConcurrentDictionary<string, FileOffset> Offsets;

        public FileDb()
        {
            Offsets = new ConcurrentDictionary<string, FileOffset>();
        }

        public async Task<bool> AddOrUpdateAsync(string collection, string key, string value)
        {
            string offsetFileName = GetOffsetFileName(collection, key);
            string dataFileName = GetDataFileName(collection, key);

            await ReadOffsetFileAync(offsetFileName); 



            return true;
        }

        public async Task<string> Get(string collection, string key)
        {
            string offsetFileName = GetOffsetFileName(collection, key);
            string dataFileName = GetDataFileName(collection, key);

            await ReadOffsetFileAync(offsetFileName);

            throw new System.NotImplementedException();
        }

        private string GetDataFileName(string collection, string key) => FilePath + DataFilePrefix + collection.ToLower();
        private string GetOffsetFileName(string collection, string key) => FilePath + OffsetFilePrefix + collection.ToLower();

        private async Task ReadOffsetFileAync(string filename)
        {
            if (Offsets.Count != 0) return;
            
            var fileContents = await File.ReadAllTextAsync(filename);

            var offsetArray = JsonSerializer.Deserialize<FileOffset[]>(fileContents);

            foreach(var offset in offsetArray)            
            {
                Offsets.TryAdd(offset.Key, offset);
            }
        }

    }

    struct FileOffset
    {
        public FileOffset(string key, long startOffset, long endOffset)
        {
            Key = key;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public string Key { get ;}
        public long StartOffset { get; }
        public long EndOffset { get; }
    }
}