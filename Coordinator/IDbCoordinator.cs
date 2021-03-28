using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace Coordinator
{
    public interface IDbCoordinator
    {
        void EnrollReplica(string host);
        void RemoveReplica(string host);

        string GetAsync(string key);
        Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values);
        Task<string> GetAsync(string collection, string key);
        Task<bool> RemoveAsync(string collection, string key);
    }

    public class DbCoordinator : IDbCoordinator
    {
        private static readonly Dictionary<string, IReplica> Replicas = new Dictionary<string, IReplica>();

        public Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values)
        {
            throw new NotImplementedException();
        }

        public void EnrollReplica(string host)
        {
            Replicas[host] = new Replica();
        }
        
        public void RemoveReplica(string host)
        {
            Replicas.Remove(host);
        }

        public string GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAsync(string collection, string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(string collection, string key)
        {
            throw new NotImplementedException();
        }
    }
}
