using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace StorageEngine
{
    public interface IFileDb
    {
        Task<string> GetAsync(string collection, string key);
        Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values);
        Task<bool> RemoveAsync(string collection, string key);
    }

    public class FileDb : IFileDb
    {
        private static readonly string SchemaFileName = "./schema.json";
        private readonly Schema Schema;
        private ConcurrentDictionary<string, IFileDbCollection> Collections;

        public FileDb()
        {
            Schema = JsonSerializer.Deserialize<Schema>(File.ReadAllText(SchemaFileName));
            Collections = new ConcurrentDictionary<string, IFileDbCollection>();

            foreach (var collection in Schema.Collections)
            {
                Collections.AddOrUpdate(collection, new FileDbCollection(collection), (x, y) => y);
            }
        }

        public Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values)
        {
            if (!Collections.ContainsKey(collection)) throw new ArgumentException("Invalid collection name");

            return Collections[collection].AddOrUpdateAsync(keys, values);
        }

        public Task<string> GetAsync(string collection, string key)
        {
            if (!Collections.ContainsKey(collection)) throw new ArgumentException("Invalid collection name");

            return Collections[collection].GetAsync(key);
        }

        public Task<bool> RemoveAsync(string collection, string key)
        {
            if (!Collections.ContainsKey(collection)) throw new ArgumentException("Invalid collection name");

            return Collections[collection].RemoveAsync(key);
        }
    }

    public class Schema
    {
        public string[] Collections { get; set; }

        //indexes

        //databases
    }
}
