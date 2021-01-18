using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleTransposition.Models
{
    public class DoubleTranspositionModel
    {
        public Stream Stream { get; set; } = default!;
        public string FileName { get; set; } = string.Empty;
        public IList<int> ColumnKeys { get; set; }
        public IList<int> RowKeys { get; set; }
    }
}
