// See https://aka.ms/new-console-template for more information
using Magisterka.Data;
using System.Diagnostics;

Console.WriteLine("Hello, World!");


DataGenerator generator = new DataGenerator();
Stopwatch stopwatch = Stopwatch.StartNew();
generator.GenerateMixed(10000000);
stopwatch.Stop();
Console.WriteLine("done in "+stopwatch.ToString());
