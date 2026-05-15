using Parquet;
using Parquet.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Magisterka.CSV
{
    public static class TestParquet
    {
        public static void WriteParquet<T>(string path, IEnumerable<T> data)
        {
            var dataList = data.ToList();
            Exception threadEx = null;

            var thread = new Thread(() =>
            {
                try
                {
                    using var fs = File.Create(path + ".parquet");
                    ParquetSerializer.SerializeAsync(dataList, fs).GetAwaiter().GetResult();
                }
                catch (Exception ex) { threadEx = ex; }
            });

            thread.Start();
            thread.Join();
            if (threadEx != null) throw threadEx;
        }

        public static List<T> ReadParquet<T>(string path) where T : class, new()
        {
            Exception threadEx = null;
            List<T> result = null;

            var thread = new Thread(() =>
            {
                try
                {
                    using var fs = File.OpenRead(path + ".parquet");
                    result = (List<T>?)ParquetSerializer.DeserializeAsync<T>(fs).GetAwaiter().GetResult();
                }
                catch (Exception ex) { threadEx = ex; }
            });

            thread.Start();
            thread.Join();
            if (threadEx != null) throw threadEx;
            return result;
        }
    }
}
