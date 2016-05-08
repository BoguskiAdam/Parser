using Parser.Models;
using System.Collections.Generic;

namespace Parser
{
    public static class ProgramsHolder
    {
        public static List<ExcelOutputModel> excelOutput = new List<ExcelOutputModel>();
        private static List<TvProgramModel> _programModels = new List<TvProgramModel>();
        public static List<TvProgramModel> ProgramModel
        {
            get
            {
                return _programModels;
            }
            set
            {
                _programModels = value;
            }
        }



        public static void AddProgram(TvProgramModel tvProgramModel)
        {
            _programModels.Add(tvProgramModel);

        }
        public static List<TvProgramModel> GetList()
        {
            return _programModels;
        }
    }
}
