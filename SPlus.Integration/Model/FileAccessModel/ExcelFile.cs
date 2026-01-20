using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class ExcelFile
    {
        public string ExcelFileId { get; set; }
        public string ExcelFileName { get; set; }
        public List<ExcelFileSheet> ExcelFileSheets { get; set; }
    }
}
