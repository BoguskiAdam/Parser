using Microsoft.Office.Interop.Excel;
using Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    static class ExcelExporter
    {
        private static List<string> _columnNames = new List<string>();

        public static List<string> ColumnNames
        {
            get { return _columnNames; }
            set { _columnNames = value; }
        }

        public static void SaveAs(string path, List<ExcelOutputModel> outputObj)
        {

            var data = new object[outputObj.Count + 1, _columnNames.Count];
            var excel = new Application();
            excel.DisplayAlerts = false;

            var workbooks = excel.Workbooks;
            var workbook = workbooks.Add(Type.Missing);
            var worksheets = workbook.Sheets;
            var worksheet = (Worksheet)worksheets[1];

            var iterator = 0;
            var column = 0;
            for(column=0;column<_columnNames.Count;column++)
            {
                data[iterator, column] = _columnNames[column];
            }

            iterator++;
            foreach (var single in outputObj)
            {
                for (column = 0; column < single.Row.Count(); column++)
                {
                    data[iterator, column] = single.Row.ElementAt(column);
                }

                iterator++;
            }

            var startCell = (Range)worksheet.Cells[1, 1];
            var endCell = (Range)worksheet.Cells[iterator, 7];
            var writeRange = worksheet.Range[startCell, endCell];

            writeRange.Value2 = data;
            writeRange.Worksheet.SaveAs(path);
            excel.Quit();
            Console.WriteLine("Complete");
            Console.WriteLine($"Saved: {outputObj.Count} rows.");
            Console.ReadKey();
        }
    }
}
