using Magisterka.CSV;
using Magisterka.Data;
using Magisterka.Parquet;
using System.Diagnostics;

namespace Magisterka.Test
{
    public class Testing
    {
        public static BenchmarkResult RunTest<T>(List<T> data, string path, Action<string, IEnumerable<T>> write, Func<string, List<T>> read)
        {
            var result = new BenchmarkResult();

            var process = Process.GetCurrentProcess();
            long memoryBefore = process.WorkingSet64;

            var sw = Stopwatch.StartNew();
            write(path, data);
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            sw.Restart();
            var readData = read(path);
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            long memoryAfter = process.WorkingSet64;
            result.MemoryUsedBytes = memoryAfter - memoryBefore;

            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
            result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);

            return result;
        }

        public static async Task<BenchmarkResult> RunTestAsync<T>(List<T> data, string path, Func<string, IEnumerable<T>, Task> write, Func<string, Task<List<T>>> read)
        {
            var result = new BenchmarkResult();

            var process = Process.GetCurrentProcess();
            long memoryBefore = process.WorkingSet64;

            var sw = Stopwatch.StartNew();
            await write(path, data);
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            sw.Restart();
            var readData = await read(path);
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            long memoryAfter = process.WorkingSet64;
            result.MemoryUsedBytes = memoryAfter - memoryBefore;

            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
            result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);

            return result;
        }
        public static async void runXcsvAsync<T>(List<T> data, string path, int x) where T : class, new()
        {
            //Warm up
            for (int i = 0; i < 15; i++)
            {
                BenchmarkResult csvResult = await Testing.RunTestAsync(
                data,
                path,
                TestParquet.WriteParquet,
                TestParquet.ReadParquet<T>
            );
            }
                List<BenchmarkResult> results = new List<BenchmarkResult>();
                for (int i = 0; i < x; i++)
                {
                    BenchmarkResult csvResult = await Testing.RunTestAsync(
                    data,
                    path,
                    TestParquet.WriteParquet,
                    TestParquet.ReadParquet<T>
                );
                    results.Add(csvResult);
                }
                TestCsv.WriteCsv("parquetResult" + path + ".csv", results);


        }
        public static void runXcsv<T>(List<T> data, string path, int x)
        {
            //Warm up
            for (int i = 0; i < 15; i++)
            {
                BenchmarkResult csvResult = Testing.RunTest(
                data,
                path,
                TestCsv.WriteCsv,
                 TestCsv.ReadCsv<T>
            );


                List<BenchmarkResult> results = new List<BenchmarkResult>();
                for (i = 0; i < x; i++)
                {
                    csvResult = Testing.RunTest(
                    data,
                    path,
                    TestCsv.WriteCsv,
                     TestCsv.ReadCsv<T>
                );
                    results.Add(csvResult);
                }
                TestCsv.WriteCsv("csvResult" + path + ".csv", results);
            }

        }
    }
}
