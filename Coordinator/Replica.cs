using System.Net.Http;
using System.Threading.Tasks;

namespace Coordinator
{
    public class Replica : IReplica
    {

        private static readonly HttpClient Client = new HttpClient();
        public Replica()
        {

        }
        
        public string Host { get; set; }
        public async Task<bool> AddOrUpdateAsync(string collection, string[] keys, string[] values)
        {
            return true;
        }

        public async Task<string> GetAsync(string collection, string key)
        {
            return "";
        }

        public async Task<bool> RemoveAsync(string collection, string key)
        {
            return true;
        }
    }
}
