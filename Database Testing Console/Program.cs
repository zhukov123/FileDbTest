using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using StorageEngine;

namespace Database_Testing_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileDb = new FileDb();

            //var value = fileDb.GetAsync("d8b537b3-358e-4686-ad83-7607324ab2a1").Result;

            string data = File.ReadAllText("./movies.json");
            var dataElements = JsonSerializer.Deserialize<Movie[]>(data, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            foreach (var movie in dataElements)
            {
                movie.Id = Guid.NewGuid();
            }

            var sw = new Stopwatch();
            
            for (var i = 0; i < 10; i++)
            {
                sw.Start();
                
                for (var j = 0; j< dataElements.Length; j += 1)
                {
                    var elements = dataElements.Skip(j).Take(1).ToArray();
                    var values = elements.Select(x => JsonSerializer.Serialize(x)).ToArray();
                    var keys = elements.Select(x => x.Id.ToString()).ToArray();
                    _ = fileDb.AddOrUpdateAsync("Movies", keys, values).Result;
                }

                Console.WriteLine($"Time ms: {sw.Elapsed.TotalMilliseconds}");
                sw.Reset();
            }

            Console.WriteLine($"Time ms: {sw.Elapsed.TotalMilliseconds}");

            // var fileDb = new FileDbCollection("TestCollection");

            // _ = fileDb.AddOrUpdateAsync("hello", "{\"key\":\"hello\", \"value\":\"something\"}").Result;
            // _ = fileDb.AddOrUpdateAsync("hello1", "{\"key\":\"hello1\", \"value\":\"something2\"}").Result;
            // _ = fileDb.AddOrUpdateAsync("hello2", "{\"key\":\"hello2\", \"value\":\"something3\"}").Result;

            // var value = fileDb.GetAsync("hello1").Result;
            // var value1 = fileDb.GetAsync("hello").Result;
            // var value2 = fileDb.GetAsync("hello2").Result;

            // _ = fileDb.AddOrUpdateAsync("hello2", "{\"key\":\"hello2\", \"value\":\"an updated value\"}").Result;

            // var value3 = fileDb.GetAsync("hello2").Result;

            // _ = fileDb.RemoveAsync("hello").Result;

            // var value4 = fileDb.GetAsync("hello").Result;

            Console.WriteLine("Please press enter!");
            System.Threading.Thread.Sleep(100000);
        }
    }

    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string[] Cast { get; set; }
        public string[] Genres { get; set; }
    }
}
