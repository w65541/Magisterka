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


            result.WriteMemoryUsedBytes = MeasurePeakMemory(() => write(path, data));
            
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;

            var fileSize = new FileInfo(path).Length;
            result.FileSizeBytes = fileSize;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            process.Refresh();

            sw.Restart();
            result.ReadMemoryUsedBytes = MeasurePeakMemory(() => read(path));
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            //result.ReadMemoryUsedBytes = memoryBefore - peakRead;
           // result.WriteMemoryUsedBytes = memoryBefore - peakWrite;
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
            result.WriteMemoryUsedBytes = MeasurePeakMemory(() => TestParquet.WriteParquet(path, data));
            sw.Stop();

            result.WriteTimeMs = sw.ElapsedMilliseconds;
            var fileSize = new FileInfo(path+".parquet").Length;
            result.FileSizeBytes = fileSize;
            long memoryBeforeRead = process.WorkingSet64;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            process.Refresh();

            sw.Restart();
            result.ReadMemoryUsedBytes = MeasurePeakMemory(() => TestParquet.ReadParquet<T>(path));
            sw.Stop();

            result.ReadTimeMs = sw.ElapsedMilliseconds;

            double fileSizeMB = fileSize / (1024.0 * 1024.0);

            //Console.WriteLine($"Memory before: {memoryBeforeRead} bytes, Memory after: {peakMemoryRead} bytes");
            result.WriteThroughput = fileSizeMB / (result.WriteTimeMs / 1000.0);
            result.ReadThroughput = fileSizeMB / (result.ReadTimeMs / 1000.0);
            return result;
        }
           
        public static void runXcparquet<T>(List<T> data, string path,int x) where T : class, new()
        {
            
            try
            {

           
            Console.WriteLine("Start parquet "+path);

            //Warm up
            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine($"Start parquet warm up {i}/15");
                BenchmarkResult csvResult = Testing.RunParquetTest<T>(
                data,
                "WriteParTest"
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
                "WriteParTest"
                );
                    results.Add(csvResult);
                Console.WriteLine($"Test {i + 1}/{x} completed for {path}");
                }
                TestCsv.WriteCsv( path + ".csv", results);
                return;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void runXcsv<T>(List<T> data, string path, int x)
        {
            Console.WriteLine("Start csv "+path);
            //Warm up
            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine($"Start csv warm up {i}/15");
                BenchmarkResult csvResult = Testing.RunTest(
                data,
                "WriteCsvTest",
                TestCsv.WriteCsv,
                 TestCsv.ReadCsv<T>
            );
                Console.WriteLine($"Start csv warm up {i}/15");
            }
            Console.WriteLine("End csv warm up");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            List<BenchmarkResult> results = new List<BenchmarkResult>();
                for (int i = 0; i < x; i++)
                {
                    var result = Testing.RunTest(
                    data,
                    "WriteCsvTest",
                    TestCsv.WriteCsv,
                     TestCsv.ReadCsv<T>
                );
                    results.Add(result);
                    Console.WriteLine($"Test {i + 1}/{x} completed for {path}");
                
            }
                TestCsv.WriteCsv( path + ".csv", results);
            }

        public static long MeasurePeakMemory(Action action)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long before = GC.GetTotalAllocatedBytes(precise: true);
            action();
            long after = GC.GetTotalAllocatedBytes(precise: true);

            return after - before;
        }
    }
}
