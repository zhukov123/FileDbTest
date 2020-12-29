using System;
using System.IO;

namespace Database_Testing_Console
{
    class Program
    {
        static void Main(string[] args)
        {

            File.WriteAllText("./temp.txt","Hello");

            Console.WriteLine("Hello World!");
        }
    }
}
