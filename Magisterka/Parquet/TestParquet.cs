using Parquet;
using Parquet.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Magisterka.Parquet
{
    public static class TestParquet
    {
        public static async Task WriteParquet<T>(string path, IEnumerable<T> data)
        { 

            var parq= await ParquetSerializer.SerializeAsync(data,path);


        }

        public static async Task<List<T>> ReadParquet<T>(string path) where T : class, new()
        {
            List<T> data = (List<T>)await ParquetSerializer.DeserializeAsync<T>(path);
            return data;
        }
        
    }
}
