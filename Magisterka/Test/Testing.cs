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


            long peakWrite = MeasurePeakMemory(() =>
            {
                write(path, data);
            });
            
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            sw.Restart();
            long peakRead = MeasurePeakMemory(() =>
            {
                var readData = read(path);
            });
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            long memoryAfter = process.WorkingSet64;
            result.ReadMemoryUsedBytes = memoryAfter - peakRead;
            result.WriteMemoryUsedBytes = memoryAfter - peakWrite;
            Console.WriteLine($"Memory before: {memoryBefore / (1024.0 * 1024.0)} bytes, Memory after: {peakRead / (1024.0 * 1024.0)} bytes");
            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
            result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);

            return result;
        }

        public static async Task<BenchmarkResult> RunTestAsync<T>(List<T> data, string path, Func<string, IEnumerable<T>, Task> write, Func<string, Task<List<T>>> read)
        {
            var result = new BenchmarkResult();

            var process = Process.GetCurrentProcess();
            long memoryBeforeWrite = process.WorkingSet64;

            var sw = Stopwatch.StartNew();

            long peakMemoryWrite = await MeasurePeakMemoryAsync(async () =>
            {
                await write(path, data);
            });

            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            sw.Restart();
            long memoryBeforeRead = process.WorkingSet64;
            long peakMemoryRead = await MeasurePeakMemoryAsync(async () =>
            {
                var readData = await read(path);
            });

            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            result.ReadMemoryUsedBytes = peakMemoryRead - memoryBeforeRead;
            result.WriteMemoryUsedBytes = memoryBeforeWrite - memoryBeforeRead;
            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            Console.WriteLine($"Memory before: {memoryBeforeRead} bytes, Memory after: {peakMemoryRead} bytes");
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

        public static async Task<long> MeasurePeakMemoryAsync(Func<Task> action)
        {
            var process = Process.GetCurrentProcess();
            long peakMemory = process.WorkingSet64;

            using var cts = new CancellationTokenSource();

            var monitoring = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    long current = process.WorkingSet64;
                    if (current > peakMemory)
                        peakMemory = current;

                    await Task.Delay(1);
                }
            });

            // wykonanie właściwej operacji
            await action();

            // zatrzymanie monitoringu
            cts.Cancel();

            // czekamy aż task się zakończy
            await monitoring;

            return peakMemory;
        }

        public static long MeasurePeakMemory(Action action)
        {
            var process = Process.GetCurrentProcess();
            long peakMemory = process.WorkingSet64;

            using var cts = new CancellationTokenSource();

            var monitoringThread = new Thread(() =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    long current = process.WorkingSet64;
                    if (current > peakMemory)
                        peakMemory = current;

                    Thread.Sleep(1); // próbkowanie co 1 ms
                }
            });

            monitoringThread.Start();

            // wykonanie operacji
            action();

            // zatrzymanie monitoringu
            cts.Cancel();

            // czekamy aż wątek się zakończy
            monitoringThread.Join();

            return peakMemory;
        }
    }
}
