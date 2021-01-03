using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace StorageEngine
{
    public interface IFileDbCollection
    {
        Task<string> GetAsync(string key);
        Task<bool> AddOrUpdateAsync(string[] keys, string[] values);
        Task<bool> AddOrUpdateWriteThroughAsync(string key, string value);
        Task<bool> AddOrUpdateWriteThroughBatchAsync(string[] keys, string[] values);
        Task<bool> RemoveAsync(string key);
    }

    public class FileDbCollection : IFileDbCollection
    {
        private const int WRITE_BUFFER_WAIT_MS = 1000;
        private const int WRITE_BUFFER_BATCH_SIZE = 100;

        private readonly string FilePath = "./";
        private readonly string DataFilePrefix = "Data_";
        private readonly string OffsetFilePrefix = "Offset_";
        private readonly string CollectionName;

        private readonly string OffsetFileName;
        private readonly string DataFileName;

        private ConcurrentDictionary<string, FileOffset> Offsets;
        private Dictionary<string, long> FilePointers;
        private ConcurrentQueue<Entry> WriteBuffer;

        public FileDbCollection(string collectionName)
        {
            CollectionName = collectionName;
            Offsets = new ConcurrentDictionary<string, FileOffset>();
            FilePointers = new Dictionary<string, long>();
            WriteBuffer = new ConcurrentQueue<Entry>();

            var timer = new Timer(WRITE_BUFFER_WAIT_MS);
            timer.Elapsed += async (x, y) =>
            {
                timer.Stop();
                await ProcessWriteQueueAsync();
                timer.Start();
            };

            timer.Start();

            OffsetFileName = GetOffsetFileName(CollectionName);
            DataFileName = GetDataFileName(CollectionName);

            ReadOffsetFileAync(OffsetFileName).GetAwaiter().GetResult();
        }

        public async Task<bool> AddOrUpdateAsync(string[] keys, string[] values)
        {
            if (keys.Length != values.Length) throw new ArgumentException("Keys and values must have the same number of elements");

            if (keys.Length == 0) return false;

            for (var i = 0; i < keys.Length; i++)
            {
                WriteBuffer.Enqueue(new Entry {
                    Key = keys[i],
                    Value = values[i]
                });
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> AddOrUpdateWriteThroughAsync(string key, string value)
        {
            var entry = value + Environment.NewLine;//JsonSerializer.Serialize(new Entry(key, value));
            var length = entry.Length;

            var fileLength = await GetFilePointerAsync();
            IncrementFilePointer(fileLength, length);

            var offset = new FileOffset(key, fileLength, length);
            Offsets[key] = offset;
            await File.AppendAllTextAsync(DataFileName, entry);
            await File.AppendAllTextAsync(OffsetFileName, JsonSerializer.Serialize(offset) + Environment.NewLine);

            return true;
        }

        public async Task<bool> AddOrUpdateWriteThroughBatchAsync(string[] keys, string[] values)
        {
            if (keys.Length != values.Length) throw new ArgumentException("Keys and values must have the same number of elements");

            if (keys.Length == 0) return false;

            var fileLength = await GetFilePointerAsync();
            long totalLength = 0;
            var offsetBuilder = new StringBuilder();
            var dataBuilder = new StringBuilder();

            for (var i = 0; i < keys.Length; i++)
            {
                var entry = values[i] + Environment.NewLine;//JsonSerializer.Serialize(new Entry(key, value));
                var length = entry.Length;
                var key = keys[i];

                var offset = new FileOffset(key, fileLength + totalLength, length);
                Offsets[key] = offset;

                offsetBuilder.Append(JsonSerializer.Serialize(offset) + Environment.NewLine);
                dataBuilder.Append(entry);
                totalLength += length;
            }

            await File.AppendAllTextAsync(DataFileName, dataBuilder.ToString());
            await File.AppendAllTextAsync(OffsetFileName, offsetBuilder.ToString());
            IncrementFilePointer(fileLength, totalLength);

            return true;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (!Offsets.ContainsKey(key)) return false;

            var offset = Offsets[key];
            offset.StartOffset = -1;

            await File.AppendAllTextAsync(OffsetFileName, JsonSerializer.Serialize(offset) + Environment.NewLine);

            return true;
        }

        public async Task<string> GetAsync(string key)
        {
            if (!Offsets.ContainsKey(key)) return null;

            var offset = Offsets[key];

            if (offset.StartOffset == -1) return null;

            var result = new byte[offset.Length];
            using (FileStream fs = File.OpenRead(DataFileName))
            {
                fs.Seek(offset.StartOffset, SeekOrigin.Begin);
                await fs.ReadAsync(result, 0, offset.Length);

                fs.Close();
            }

            var str = System.Text.Encoding.Default.GetString(result);

            return str.TrimEnd();
        }


        private async Task ProcessWriteQueueAsync()
        {
            var entries = new List<Entry>();

            while (WriteBuffer.TryDequeue(out var entry))
            {
                entries.Add(entry);

                if (entries.Count >= WRITE_BUFFER_BATCH_SIZE)
                {
                    await ProcessEntries(entries);

                    Console.WriteLine($"Write buffer writing {entries.Count} entries");

                    entries.Clear();
                }
            }

            if (entries.Count > 1)
            {
                Console.WriteLine($"Write buffer writing {entries.Count} entries");
            }

            await ProcessEntries(entries);
        }

        private Task ProcessEntries(IEnumerable<Entry> entries)
        {
            var entriesArr = entries.ToArray();

            var keys = entriesArr.Select(x => x.Key).ToArray();
            var values = entriesArr.Select(x => x.Value).ToArray();

            return AddOrUpdateWriteThroughBatchAsync(keys, values);
        }

        private async Task<long> GetFilePointerAsync()
        {
            return FilePointers.ContainsKey(DataFileName)
                ? FilePointers[DataFileName]
                : File.Exists(DataFileName) ? await Task.FromResult(new FileInfo(DataFileName).Length) : 0;
        }

        private void IncrementFilePointer(long fileLength, long length)
        {
            FilePointers[DataFileName] = fileLength + length;
        }

        private string GetDataFileName(string collection) => FilePath + DataFilePrefix + collection.ToLower() + ".json";
        private string GetOffsetFileName(string collection) => FilePath + OffsetFilePrefix + collection.ToLower() + ".json";

        //warning need to make this thread safe
        private async Task ReadOffsetFileAync(string filename)
        {
            if (Offsets.Count != 0) return;

            if (!File.Exists(filename)) return;

            var fileContents = await File.ReadAllLinesAsync(filename);

            var offsetArray = fileContents.Select(x => JsonSerializer.Deserialize<FileOffset>(x,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })).ToArray();

            foreach (var offset in offsetArray)
            {
                Offsets.AddOrUpdate(offset.Key, offset, (key, value) => offset);
            }
        }


    }
}

//TODO: if offsets file has a stale entry, remove it
