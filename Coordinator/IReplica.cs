using System.Threading.Tasks;

namespace Coordinator
{
    public interface IReplica
    {
        string Host { get; }
        Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values);
        Task<string> GetAsync(string collection, string key);
        Task<bool> RemoveAsync(string collection, string key);
    }
}
