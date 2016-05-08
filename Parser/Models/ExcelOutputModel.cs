using System.Collections.Generic;
using Parser.Helpers;

namespace Parser.Models
{
    public class ExcelOutputModel : IExcelOutput
    {
        public ExcelOutputModel()
        {
            Row = new List<string>();
        }
        public List<string> Row { get; set; }
    }
}
