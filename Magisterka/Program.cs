// See https://aka.ms/new-console-template for more information
using Magisterka.CSV;
using Magisterka.Data;
using Magisterka.Test;
using Parquet.Serialization;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Hello, World!");
SynchronizationContext.SetSynchronizationContext(null);
try
{
    using var ms = new MemoryStream();
    await ParquetSerializer.SerializeAsync(new List<RecordInt>(), ms);
    ms.Position = 0;
    await ParquetSerializer.DeserializeAsync<RecordInt>(ms);
}
catch { }

DataGenerator generator = new DataGenerator();
Stopwatch stopwatch = Stopwatch.StartNew();
List<RecordInt> dataInt=generator.GenerateInt(10000000);
stopwatch.Stop();
Console.WriteLine("done in "+stopwatch.ToString());
List<BenchmarkResult> csv=new List<BenchmarkResult>();

/*
for (int i = 0; i < 5; i++)
{
    BenchmarkResult csvResult = Testing.RunTest(
    dataInt,
    "data.csv",
    TestCsv.WriteCsv,
     TestCsv.ReadCsv<RecordInt>
);
    csv.Add(csvResult);
}
TestCsv.WriteCsv("csvResult.csv", csv);


var result = await Testing.RunTestAsync(
    dataInt,
    "test.parquet",
    TestParquet.WriteParquet,
    TestParquet.ReadParquet<RecordInt>
);
Console.WriteLine($"Parquet - Write Time: {result.WriteTimeMs} ms, Read Time: {result.ReadTimeMs} ms, File Size: {result.FileSizeBytes} bytes, Memory Used: {result.ReadMemoryUsedBytes} bytes, Write Throughput: {result.WriteThroughput} MB/s, Read Throughput: {result.ReadThroughput} MB/s");
*/
intTest();
async void intTest()
{
    //Testing.runXcsv<RecordInt>(dataInt, "TestResultCsvInt", 10);
    await Testing.runXcparquet<RecordInt>(dataInt, "TestResultParquetInt", 10
               );
  
}