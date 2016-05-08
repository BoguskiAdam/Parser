namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            TelemagazynParser telemagazynParser = new TelemagazynParser();
            telemagazynParser.StartParsing();

            HolidayParser.HolidayParser holidayDatesParser = new HolidayParser.HolidayParser();
            holidayDatesParser.Parse(Settings.startData.Year,Settings.endData.Year);
        }
    }
}
