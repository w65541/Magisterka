// See https://aka.ms/new-console-template for more information
using Magisterka.CSV;
using Magisterka.Data;
using Magisterka.Parquet;
using Magisterka.Test;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Hello, World!");


DataGenerator generator = new DataGenerator();
Stopwatch stopwatch = Stopwatch.StartNew();
List<RecordInt> data=generator.GenerateInt(10000000);
stopwatch.Stop();
Console.WriteLine("done in "+stopwatch.ToString());
List<BenchmarkResult> csv=new List<BenchmarkResult>();
for (int i = 0; i < 5; i++)
{
    BenchmarkResult csvResult = Testing.RunTest(
    data,
    "data.csv",
    TestCsv.WriteCsv,
     TestCsv.ReadCsv<RecordInt>
);
    csv.Add(csvResult);
}
TestCsv.WriteCsv("csvResult.csv", csv);

var result = await Testing.RunTestAsync(
    data,
    "test.parquet",
    TestParquet.WriteParquet,
    TestParquet.ReadParquet<RecordInt>
);
Console.WriteLine($"Parquet - Write Time: {result.WriteTimeMs} ms, Read Time: {result.ReadTimeMs} ms, File Size: {result.FileSizeBytes} bytes, Memory Used: {result.MemoryUsedBytes} bytes, Write Throughput: {result.WriteThroughput} MB/s, Read Throughput: {result.ReadThroughput} MB/s");