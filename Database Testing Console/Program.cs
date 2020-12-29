﻿using System;
using System.IO;
using System.Text.Json;

namespace Database_Testing_Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var fileDb = new FileDbCollection("TestCollection");
            // var str = File.ReadAllText("./Offset_testcollection.json");
            // var fileOffset = JsonSerializer.Deserialize<FileOffset>(str, 
            // new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            
            // _ = fileDb.AddOrUpdateAsync("hello", "{\"key\":\"hello\", \"value\":\"something\"}").Result;
            // _ = fileDb.AddOrUpdateAsync("hello1", "{\"key\":\"hello1\", \"value\":\"something2\"}").Result;
            // _ = fileDb.AddOrUpdateAsync("hello2", "{\"key\":\"hello2\", \"value\":\"something3\"}").Result;

            var value = fileDb.GetAsync("hello1").Result;
            var value1 = fileDb.GetAsync("hello").Result;
            var value2 = fileDb.GetAsync("hello2").Result;

            Console.WriteLine("Hello World!");
        }
    }
}
