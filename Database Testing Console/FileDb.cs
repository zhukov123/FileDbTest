using System.Collections.Concurrent;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Database_Testing_Console
{
    interface IFileDbCollection
    {
        Task<string> GetAsync(string key);
        Task<bool> AddOrUpdateAsync(string key, string value);

    }

    class FileDbCollection : IFileDbCollection
    {
        private readonly string FilePath = "./";
        private readonly string DataFilePrefix = "Data_";
        private readonly string OffsetFilePrefix = "Offset_";
        private readonly string CollectionName;

        private ConcurrentDictionary<string, FileOffset> Offsets;

        public FileDbCollection(string collectionName)
        {
            CollectionName = collectionName;
            Offsets = new ConcurrentDictionary<string, FileOffset>();
        }

        public async Task<bool> AddOrUpdateAsync(string key, string value)
        {
            var offsetFileName = GetOffsetFileName(CollectionName, key);
            var dataFileName = GetDataFileName(CollectionName, key);

            await ReadOffsetFileAync(offsetFileName);

            var entry = value;//JsonSerializer.Serialize(new Entry(key, value));
            var length = entry.Length;
            //var endOfFile = File.

            using (FileStream fs = File.OpenWrite(dataFileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                await fs.WriteAsync(info, 0, length, new System.Threading.CancellationToken());

                fs.Close();
            }

            return true;
        }

        public async Task<string> GetAsync(string key)
        {
            var offsetFileName = GetOffsetFileName(CollectionName, key);
            var dataFileName = GetDataFileName(CollectionName, key);

            await ReadOffsetFileAync(offsetFileName);

            throw new System.NotImplementedException();
        }

        private string GetDataFileName(string collection, string key) => FilePath + DataFilePrefix + collection.ToLower() + ".json";
        private string GetOffsetFileName(string collection, string key) => FilePath + OffsetFilePrefix + collection.ToLower() + ".json";

        //warning need to make this thread safe
        private async Task ReadOffsetFileAync(string filename)
        {
            if (Offsets.Count != 0) return;

            var fileContents = await File.ReadAllLinesAsync(filename);

            var offsetArray = fileContents.Select(x => JsonSerializer.Deserialize<FileOffset>(x,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })).ToArray();

            foreach (var offset in offsetArray)
            {
                Offsets.AddOrUpdate(offset.Key, offset);
            }
        }
    }

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

    public class FileOffset
    {
        public FileOffset(string key, long startOffset, long endOffset)
        {
            Key = key;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public string Key { get; }
        public long StartOffset { get; }
        public long EndOffset { get; }
    }
}