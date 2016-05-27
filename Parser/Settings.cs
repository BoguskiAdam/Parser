using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static class Settings
    {
        //TV program parser
        public static string TvProgramSiteUrl = "http://www.telemagazyn.pl/";
        public static string TvProgramDayPrefix = "canal/?dzien=";

        //HolidayParser
        public static string HolidaySiteUrl = @"http://www.kalendarzswiat.pl/lista_swiat/";

        public static string TvProgramExcelPath = @"C:\Test\TeleMagazynParsed.xlsx";
        public static string DatesExcelPath = @"C:\Test\DatesParsed.xlsx";

        public static string TvProgramCSVPath = @"C:\Test\TeleMagazynParsed.csv";
        public static string DatesCSVPath = @"C:\Test\DatesParsed.csv";

        public static DateTime startData = new DateTime(2012, 01, 01);
        public static DateTime endData = new DateTime(2012, 01, 05);
    }
}
