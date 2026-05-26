using Magisterka.CSV;
using Magisterka.Data;
using System;
using System.Diagnostics;

namespace Magisterka.Test
{
    public class Testing
    {
        public static BenchmarkResult RunTest<T>(List<T> data, string path, Action<string, IEnumerable<T>> write, Func<string, List<T>> read)
        {
            var result = new BenchmarkResult();

            var process = Process.GetCurrentProcess();
            long memoryBefore = GC.GetTotalMemory(false); //process.WorkingSet64;

            var sw = Stopwatch.StartNew();


            long peakWrite = MeasurePeakMemory(() =>
            {
                write(path, data);
            });
            
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            process.Refresh();

            sw.Restart();
            long peakRead = MeasurePeakMemory(() =>
            {
                var readData = read(path);
            });
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            result.ReadMemoryUsedBytes = memoryBefore - peakRead;
            result.WriteMemoryUsedBytes = memoryBefore - peakWrite;
            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
            result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);

            return result;
        }
        public static BenchmarkResult RunParquetTest<T>(List<T> data, string path) where T : class, new()
        {
            var result = new BenchmarkResult();

            var process = Process.GetCurrentProcess();
            long memoryBeforeWrite = GC.GetTotalMemory(false); // process.WorkingSet64;

            var sw = Stopwatch.StartNew();

            long peakWrite = MeasurePeakMemory(() =>
            {
                Console.WriteLine("Before write");
                TestParquet.WriteParquet(path, data);
                Console.WriteLine("After write");
            });


            sw.Stop();
            Console.WriteLine(peakWrite);
            result.WriteTimeMs = sw.ElapsedMilliseconds;
            var fileSize = new FileInfo(path+".parquet").Length;
            result.FileSizeBytes = fileSize;
            long memoryBeforeRead = process.WorkingSet64;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            process.Refresh();

            sw.Restart();
            long peakRead = MeasurePeakMemory(() =>
            {

                TestParquet.ReadParquet<T>(path);

            });
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            result.ReadMemoryUsedBytes = peakRead - memoryBeforeRead;
            result.WriteMemoryUsedBytes = peakWrite - memoryBeforeWrite;
            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            //Console.WriteLine($"Memory before: {memoryBeforeRead} bytes, Memory after: {peakMemoryRead} bytes");
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

           /* long peakMemoryWrite = await MeasurePeakMemoryAsync(async () =>
            {*/
                Console.WriteLine("Before write");
                await write(path, data);
                Console.WriteLine("After write");
            //});

            Console.WriteLine("A");
            sw.Stop();
            Console.WriteLine("B");
            result.WriteTimeMs = sw.ElapsedMilliseconds;
            Console.WriteLine("C");
            var fileSize = 123;
            result.FileSizeBytes = fileSize;
            Console.WriteLine("D");
            sw.Restart();
            Console.WriteLine("E");
            long memoryBeforeRead = process.WorkingSet64;
            Console.WriteLine("F");
            Console.WriteLine("read is null: " + (read == null));
            Console.WriteLine("read type: " + read.GetType());
            /*long peakMemoryRead = await MeasurePeakMemoryAsync2(async () =>
            {*/
            ThreadPool.GetAvailableThreads(out int workers, out int iocp);
            Console.WriteLine($"Available threads: workers={workers} iocp={iocp}");
            Console.WriteLine("Before read");
            //var readData = await TestParquet.ReadParquet<RecordInt>(path);// await read(path );
          //  var testResult = await TestParquet.ReadParquet<RecordInt>(path);
            Console.WriteLine("after read");
            /*
            });*/
            long peakMemoryRead = 1;
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
        public static void runXcparquet<T>(List<T> data, string path,int x) where T : class, new()
        {
            
            try
            {

           
            Console.WriteLine("Start parquet");
            //Warm up
            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine("Start parquet warm up");
                BenchmarkResult csvResult = Testing.RunParquetTest<T>(
                data,
                path
                );
                    /*
                    var result = new BenchmarkResult();

                    var process = Process.GetCurrentProcess();
                    long memoryBeforeWrite = process.WorkingSet64;

                    var sw = Stopwatch.StartNew();

                    long peakWrite = MeasurePeakMemory(() =>
                    {
                        Console.WriteLine("Before write");
                        TestParquet.WriteParquet(path, data);
                        Console.WriteLine("After write");
                    });

                
                    sw.Stop();
                    Console.WriteLine(peakWrite);
                    Console.WriteLine("B");
                    result.WriteTimeMs = sw.ElapsedMilliseconds;
                    Console.WriteLine("C");
                    var fileSize = 123;
                    result.FileSizeBytes = fileSize;
                    Console.WriteLine("D");
                    
                    Console.WriteLine("E");
                    long memoryBeforeRead = process.WorkingSet64;
                    Console.WriteLine("F");
                    sw.Restart();
                    long peakRead = MeasurePeakMemory(() =>
                    {
                    
                     TestParquet.ReadParquet<T>(path); 
                   
                    });
                    sw.Stop();

                    result.ReadTimeMs = sw.ElapsedMilliseconds;

                    result.ReadMemoryUsedBytes = peakRead - memoryBeforeRead;
                    result.WriteMemoryUsedBytes = peakWrite - memoryBeforeWrite;
                    double fileSizeMB = fileSize / (1024.0 * 1024.0);

                    //Console.WriteLine($"Memory before: {memoryBeforeRead} bytes, Memory after: {peakMemoryRead} bytes");
                    result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
                    result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);




                    */
                    Console.WriteLine($"Stop parquet warm up {i}/15");
            }
            Console.WriteLine("End parquet warm up");

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                List<BenchmarkResult> results = new List<BenchmarkResult>();
                for (int i = 0; i < x; i++)
                {
                    BenchmarkResult csvResult = Testing.RunParquetTest<T>(
                data,
                path
                );
                    results.Add(csvResult);
                Console.WriteLine($"Test {i + 1}/{x} completed for {path}");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                TestCsv.WriteCsv("parquetResult" + path + ".csv", results);
                return;
            }
            catch (Exception)
            {

                throw;
            }
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
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            List<BenchmarkResult> results = new List<BenchmarkResult>();
                for (int i = 0; i < x; i++)
                {
                    var result = Testing.RunTest(
                    data,
                    path,
                    TestCsv.WriteCsv,
                     TestCsv.ReadCsv<T>
                );
                    results.Add(result);
                    Console.WriteLine($"Test {i + 1}/{x} completed for {path}");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
                TestCsv.WriteCsv("csvResult" + path + ".csv", results);
            }

        public static async Task<long> MeasurePeakMemoryAsync2(Func<Task> action)
        {
            var process = Process.GetCurrentProcess();
            long peakMemory = process.WorkingSet64;

            using var cts = new CancellationTokenSource();

            var monitoring = Task.Run(async () =>
            {
                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        process.Refresh();

                        long current = process.WorkingSet64;

                        if (current > peakMemory)
                            peakMemory = current;

                        await Task.Delay(1, cts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    // normalne zakończenie
                }
            });

            await action();

            cts.Cancel();

            await monitoring;

            return peakMemory;
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long peakMemory = GC.GetTotalMemory(false);
            using var cts = new CancellationTokenSource();

            var monitoringThread = new Thread(() =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    long current = GC.GetTotalMemory(false);
                    if (current > peakMemory)
                        peakMemory = current;
                    Thread.Sleep(1);
                }
            });

            monitoringThread.Start();
            action();
            cts.Cancel();
            monitoringThread.Join();

            return peakMemory;
        }
    }
}
