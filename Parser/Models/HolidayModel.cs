using System;

namespace Parser.Models
{
    public class HolidayModel
    {
        public string Name { get; set; }
        public int Day { get; set; }
        public string DayOfWeek { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsDayOff { get; set; }
        public string ExtendedDate { get; set; }
    }
}
