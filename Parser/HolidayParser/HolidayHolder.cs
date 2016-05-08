using Parser.Models;
using System.Collections.Generic;

namespace Parser.HolidayParser
{
    public static class HolidayHolder
    {
        public static List<ExcelOutputModel> excelOutput = new List<ExcelOutputModel>();
        private static List<HolidayModel> _holidayModels = new List<HolidayModel>();

        public static List<HolidayModel> HolidayModel
        {
            get
            {
                return _holidayModels;
            }
            set
            {
                _holidayModels = value;
            }
        }
        
        public static void AddProgram(HolidayModel holidayModel)
        {
            _holidayModels.Add(holidayModel);

        }
        public static List<HolidayModel> GetList()
        {
            return _holidayModels;
        }
    }
}
