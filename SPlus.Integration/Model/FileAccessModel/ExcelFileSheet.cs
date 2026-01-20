using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class ExcelFileSheet
    {
        public string ExcelFileSheetId { get; set; }
        public string ExcelFileSheetName { get; set; }
        public List<ExcelFileSheetColumn> ExcelFileSheetColumn { get; set; }
    }
}
