using Parser.Exporter;
using Parser.Models;
using System;
using System.Reflection;

namespace Parser
{
    public class TelemagazynParser
    {
        public void StartParsing()
        {
            TvParser tvParser = new TvParser();
            DateTime startDay = Settings.startData;
            DateTime endsDay = Settings.endData;
            int comparedDates = DateTime.Compare(startDay, endsDay);

            ExcelExporter.ColumnNames.Clear();
            Type itemType = typeof(TvProgramModel);
            PropertyInfo[] properties = itemType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                ExcelExporter.ColumnNames.Add(propertyName);
            }

            FileExporter exporter = new FileExporter(Settings.TvProgramCSVPath);
            bool first = true;
            if (comparedDates <= 0)
            {
                do
                {
                    System.Console.WriteLine($"Parsing day: {String.Format("{0:yyyy-MM-dd}", startDay)}");
                    if (first)
                    {
                        tvParser.ParseTvDay(String.Format("{0:yyyy-MM-dd}", startDay), first, exporter);
                        first = false;
                    }
                    else
                        tvParser.ParseTvDay(String.Format("{0:yyyy-MM-dd}", startDay), false, exporter);
                    startDay = startDay.AddDays(1);
                } while (DateTime.Compare(startDay, endsDay) < 0);

                //ExcelExporter.SaveAs(Settings.TvProgramExcelPath,ProgramsHolder.excelOutput);
            }
        }
    }
}
