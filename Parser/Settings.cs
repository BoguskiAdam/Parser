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

        public static string ExcelPath = @"C:\Test\TeleMagazynParsed.xlsx";
        public static DateTime startData = new DateTime(2016, 05, 08);
        public static DateTime endData = new DateTime(2016, 05, 08);
    }
}
