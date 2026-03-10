using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Test
{
    public class BenchmarkResult
    {
        public long WriteTimeMs { get; set; }
        public long ReadTimeMs { get; set; }
        public double WriteThroughput { get; set; }
        public double ReadThroughput { get; set; }
        public long FileSizeBytes { get; set; }
        public long MemoryUsedBytes { get; set; }
    }
}
