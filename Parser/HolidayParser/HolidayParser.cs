using Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser.HolidayParser
{
    public class HolidayParser
    {
        public void Parse(int startYear, int stopYear)
        {
            for (int year = startYear; year <= stopYear; year++)
            {
                Console.WriteLine($"Parsing: {year}");
                ParseHolidayYear(year.ToString());
            }
            Type itemType = typeof(HolidayModel);
            PropertyInfo[] properties = itemType.GetProperties();
            ExcelExporter.ColumnNames.Clear();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                ExcelExporter.ColumnNames.Add(propertyName);
            }
            AddExcelOutput();
            ExcelExporter.SaveAs(Settings.ExcelPath, HolidayHolder.excelOutput);
        }
        private HolidayModel ParseSingleHoliday(string respondedString)
        {
            HolidayModel holidayModel = new HolidayModel();
            string prefix = "<div class=\"fcal_feast_body\">";
            string sufix = "</div>";
            string _pattern = $"(?={prefix}).+?({sufix})";
            Regex expression = new Regex(_pattern, RegexOptions.Compiled);
            MatchCollection matches = expression.Matches(respondedString);
            string match = matches[0].Value;
            holidayModel.Name = match.Substring(prefix.Length, match.Length - prefix.Length - sufix.Length);
            holidayModel.IsDayOff = respondedString.Contains("fcal_free");

            prefix = "<div class=\"fcal_feast_date\">";
            _pattern = $"(?={prefix}).+?({sufix})";
            expression =  new Regex(_pattern, RegexOptions.Compiled);
            matches = expression.Matches(respondedString);
            match = matches[0].Value;
            int indexOfEnd = match.IndexOf(" ", prefix.Length - 1);
            holidayModel.Day = int.Parse(match.Substring(prefix.Length, indexOfEnd - prefix.Length));

            return holidayModel;
        }

        private int GetMonthNumber(string monthStr)
        {
            switch(monthStr)
            {
                case "Styczeń":
                    return 1;
                case "Luty":
                    return 2;
                case "Marzec":
                    return 3;
                case "Kwiecień":
                    return 4;
                case "Maj":
                    return 5;
                case "Czerwiec":
                    return 6;
                case "Lipiec":
                    return 7;
                case "Sierpień":
                    return 8;
                case "Wrzesień":
                    return 9;
                case "Październik":
                    return 10;
                case "Listopad":
                    return 11;
                case "Grudzień":
                    return 12;
                default:
                    return -1;
            }
        }

        private void ParseMonth(string respondedString, string year)
        {
            string prefix = "<div class=\"fcal_monthname\">";
            string sufix = "</div>";
            string _pattern = $"({prefix}).+?({sufix})";
            Regex expression = new Regex(_pattern, RegexOptions.Compiled);
            MatchCollection matches = expression.Matches(respondedString);
            string month = matches[0].Value;
            month = month.Substring(prefix.Length, month.Length - sufix.Length - prefix.Length);
            Console.WriteLine($"Parsing: {year}-{month}");

            int indexOfStart = respondedString.IndexOf("</div>");
            if (indexOfStart != 0)
            {
                respondedString = respondedString.Substring(indexOfStart + 6);
                _pattern = "(<div class=\"fcal_feast).+?(</div>\\s?</div>)";
                expression = new Regex(_pattern, RegexOptions.Compiled);
                matches = expression.Matches(respondedString);

                foreach(Match match in matches)
                {
                    HolidayModel holidayModel = ParseSingleHoliday(match.Value);
                    holidayModel.Year = int.Parse(year);
                    DateTime date = new DateTime(holidayModel.Year, GetMonthNumber(month), holidayModel.Day);
                    holidayModel.ExtendedDate = date.ToShortDateString();
                    holidayModel.Month = date.Month;
                    holidayModel.DayOfWeek = date.DayOfWeek.ToString();
                    HolidayHolder.HolidayModel.Add(holidayModel);
                }
            }
        }
        public void ParseHolidayYear(string year)
        {
            WebClient holidayWebClient = new WebClient();
            holidayWebClient.Encoding = Encoding.UTF8;
            string respondedString = holidayWebClient.DownloadString(Settings.HolidaySiteUrl + year);
            string _pattern = @"(<td>).*?(</td>)";
            Regex expression = new Regex(_pattern, RegexOptions.Compiled);
            MatchCollection matches = expression.Matches(respondedString);

            foreach (Match match in matches)
            {
                ParseMonth(match.Value, year);
            }
        }

        private void AddExcelOutput()
        {
            foreach (HolidayModel model in HolidayHolder.HolidayModel)
            {
                Type itemType = model.GetType();
                PropertyInfo[] properties = itemType.GetProperties();
                ExcelOutputModel newExcelOutputModel = new ExcelOutputModel();
                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;
                    string propertyValue = string.Empty;
                    var propertyVarValue = property.GetValue(model, null);
                    newExcelOutputModel.Row.Add(propertyVarValue.ToString());
                }
                HolidayHolder.excelOutput.Add(newExcelOutputModel);
            }
        }
    }
}
