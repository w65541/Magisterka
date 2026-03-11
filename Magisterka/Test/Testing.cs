using System.Diagnostics;

namespace Magisterka.Test
{
    public class Testing
    {
        public static BenchmarkResult RunTest<T>(List<T> data,string path,Action<string, IEnumerable<T>> write,Func<string, List<T>> read)
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

        public static async Task<BenchmarkResult> RunTestAsync<T>(List<T> data,string path,Func<string, IEnumerable<T>, Task> write,Func<string, Task<List<T>>> read)
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
    }
}
