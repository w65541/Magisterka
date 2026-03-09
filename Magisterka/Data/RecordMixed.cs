using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Data
{
    public class RecordMixed
    {
        public int Id { get; set; }
        public int ValueInt { get; set; }
        public double ValueDouble { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}

