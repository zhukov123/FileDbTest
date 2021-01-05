using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageEngine;

namespace StorageEngineWeb.Controllers
{

    [ApiController]
    [Route("db")]
    public class FileDbController : ControllerBase
    {
        private IFileDb _fileDb;

        public FileDbController(IFileDb fileDb)
        {
            _fileDb = fileDb;
        }

        [HttpGet]
        [Route("{collection}/{key}")]
        public async Task<string> GetAsync(string collection, string key)
        {
            return await _fileDb.GetAsync(collection, key);
        }

        [HttpPost]
        [Route("{collection}")]
        public async Task<bool> PostAsync(string collection, DbEntry[] entry)
        {
            var keys = entry.Select(x => x.Key).ToArray();
            var values = entry.Select(x => x.Value).ToArray();

            return await _fileDb.AddOrUpdateAsync(collection, keys, values);
        }
    }
}
