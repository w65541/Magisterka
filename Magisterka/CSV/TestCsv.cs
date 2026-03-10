using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.CSV
{
    public class TestCsv
    {
        public static void WriteCsv<T>(string path, IEnumerable<T> data)
        {
            using var writer = new StreamWriter(path);
            using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(data);
        }

        public static List<T> ReadCsv<T>(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<T>().ToList();
        }
    }
}
