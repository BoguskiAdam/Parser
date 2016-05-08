using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class TvProgramModel
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public AgeCategoryEnum AgeCategory { get; set; }
        public string Url { get; set; }
        public double Duration { get; set; }
        public TimeSpan Starts { get; set; }
        public TimeSpan Ends { get; set; }
    }
}
